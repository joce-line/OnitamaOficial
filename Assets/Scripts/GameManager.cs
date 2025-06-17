using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.scripts.InfoPlayer;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    public List<UnitS> player1Units = new List<UnitS>();
    public List<UnitS> player2Units = new List<UnitS>();
    public GameObject playerPawnPrefab;
    public GameObject playerKingPrefab;

    public bool pturn = true;
    public int turnNum = -1;
    public GameObject painelReconectar;
    private bool p1KingCaptured = false;
    private bool p2KingCaptured = false;
    private GridNodeS p1GoalNode;
    private GridNodeS p2GoalNode;
    private Dictionary<int, SkinData> playerSkins = new Dictionary<int, SkinData>();
    private const byte RESET_LOBBY_EVENT = 2;

    public int activePlayer = 0;
    public TurnManager turnManager;

    public static GameManager instance;
    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            instance = (GameManager)FindFirstObjectByType(typeof(GameManager));
        }
        return instance;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void Awake()
    {
        if (instance == null)
        {
            instance = (GameManager)FindFirstObjectByType(typeof(GameManager));
            if (instance == null)
            {
                instance = this;
            }
        }
    }

    public void Start()
    {
        LoadPlayerSkins();
        InitPlayers();
        InitGoalNodes();
        EventManager.StartListening("EndPlayerMovement", OnEndPlayerMovement);
        EventManager.StartListening("TimeoutPlayerChange", OnTimeoutPlayerChange);
        RunGame();
        //musica de fundo        
        MusicManager.instance.playMusicBattle();
    }

    private void LoadPlayerSkins()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.ContainsKey("finalSkinId"))
            {
                int skinId = (int)player.CustomProperties["finalSkinId"];
                SkinData skin = FetchSkinFromDatabase(skinId);
                if (skin != null)
                {
                    playerSkins[player.ActorNumber] = skin;
                    Debug.Log($"[GameManager] Skin carregada: ActorNumber={player.ActorNumber}, skinId={skinId}, nome={skin.nome}");
                }
                else
                {
                    playerSkins[player.ActorNumber] = GetDefaultSkin();
                    Debug.LogWarning($"[GameManager] Skin não encontrada para ActorNumber={player.ActorNumber}, usando padrão.");
                }
            }
            else
            {
                playerSkins[player.ActorNumber] = GetDefaultSkin();
                Debug.LogWarning($"[GameManager] finalSkinId não definido para ActorNumber={player.ActorNumber}, usando padrão.");
            }
        }
    }

    private SkinData FetchSkinFromDatabase(int skinId)
    {
        string query = $"SELECT id_Conjunto, nome_Conjunto, caminho_Pawn, caminho_King FROM conjuntos_skins WHERE id_Conjunto = {skinId}";
        List<Dictionary<string, object>> results = DatabaseManager.Instance.ExecuteReader(query);

        if (results.Count > 0)
        {
            var row = results[0];
            try
            {
                return new SkinData
                {
                    id = (int)row["id_Conjunto"],
                    nome = row["nome_Conjunto"]?.ToString() ?? "",
                    caminhoPawn = row["caminho_Pawn"]?.ToString() ?? "",
                    caminhoKing = row["caminho_King"]?.ToString() ?? ""
                };
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[GameManager] Erro ao processar skin: {e.Message}");
                return null;
            }
        }
        return null;
    }

    private SkinData GetDefaultSkin()
    {
        return new SkinData
        {
            id = 1,
            nome = "Default",
            caminhoPawn = "https://firebasestorage.googleapis.com/v0/b/ihc-manut.appspot.com/o/skins%2Fpawn_1747457311263_BrancoPeao.png?alt=media&token=8a5bade5-11e6-45be-80ac-b4963b5ebd67",
            caminhoKing = "https://firebasestorage.googleapis.com/v0/b/ihc-manut.appspot.com/o/skins%2Fking_1747457312118_BrancoRei.png?alt=media&token=f1045dd7-3361-4eb4-9973-8ec14944ac7e"
        };
    }

    public void InitPlayers()
    {

        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == 2)
                {
                    CreateUnit(1, i, 0, UnitS.unitType.king);
                }
                else
                {
                    CreateUnit(1, i, 0, UnitS.unitType.pawn);
                }
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                if (i == 2)
                {
                    CreateUnit(2, i, 4, UnitS.unitType.king);
                }
                else
                {
                    CreateUnit(2, i, 4, UnitS.unitType.pawn);
                }
            }

        }
    }

    //para pegar os nós onde ficam os mestres
    public void InitGoalNodes()
    {
        p1GoalNode = GridManagerS.GetNodeS(2, 4);
        p2GoalNode = GridManagerS.GetNodeS(2, 0);
    }

    public void RunGame()
    {
        activePlayer = 1;
        StartCoroutine(setStartingActivePlayer());
    }

    IEnumerator setStartingActivePlayer()
    {
        yield return new WaitForSeconds(.5f);
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "ActivePlayer", activePlayer } });
    }

    public IEnumerator WaitForLeaveRoomAndDisconnect()
    {
        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        SceneManager.LoadScene("MenuPrincipal");
        MusicManager.instance.playMusicGeral();
    }

    public void CreateUnit(int playerId, int x, int y, UnitS.unitType utype)
    {
        string prefabName = (utype == UnitS.unitType.pawn) ? "Prefabs/PlayerPawn" : "Prefabs/PlayerKing";
        GameObject newGo = PhotonNetwork.Instantiate(prefabName, new Vector3(0, 0, -1), Quaternion.identity);

        int actorNumber = playerId == 1 ? PhotonNetwork.PlayerList[0].ActorNumber : PhotonNetwork.PlayerList[1].ActorNumber;
        SkinData skin = playerSkins.ContainsKey(actorNumber) ? playerSkins[actorNumber] : GetDefaultSkin();
        string spriteUrl = (utype == UnitS.unitType.pawn) ? skin.caminhoPawn : skin.caminhoKing;

        //string spritePath = (playerId == 1)
        //    ? (utype == UnitS.unitType.pawn ? "Skins/PretoPeao" : "Skins/PretoRei")
        //    : (utype == UnitS.unitType.pawn ? "Skins/BrancoPeao" : "Skins/BrancoRei");

        int unitId = newGo.GetComponent<PhotonView>().ViewID;

        newGo.GetComponent<PhotonView>().RPC(
            "RPC_SetData",
            RpcTarget.AllBuffered,
            unitId,
            x,
            y,
            spriteUrl
        );

        UnitS temp = newGo.GetComponent<UnitS>();
        if (playerId == 1) player1Units.Add(temp);
        else player2Units.Add(temp);
    }


    public void OnEndPlayerMovement(string eventName, ActionParams data)
    {
        int receivedPlayer = data.Get<int>("activePlayer");
        GridNodeS receivedNode = data.Get<GridNodeS>("lastSelectedNode");

        if (CheckForGameEnd(receivedNode))
        {
            int vencedor = receivedPlayer;
            int perdedor = (receivedPlayer == 1) ? 2 : 1;

            object[] content = new object[] { vencedor, perdedor };

            RaiseEventOptions options = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All
            };

            PhotonNetwork.RaiseEvent(1, content, options, ExitGames.Client.Photon.SendOptions.SendReliable);


            ActionParams temp = new ActionParams();
            temp.Put("activePlayer", activePlayer);
            EventManager.TriggerEvent("GameOver", temp);

        }
        else
        {
            activePlayer = (receivedPlayer == 1) ? 2 : 1;

            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "ActivePlayer", activePlayer } });

        }
    }

    public void OnTimeoutPlayerChange(string eventName, ActionParams data)
    {
        int receivedPlayer = data.Get<int>("activePlayer");
        activePlayer = (receivedPlayer == 1) ? 2 : 1;
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "ActivePlayer", activePlayer } });
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == RESET_LOBBY_EVENT)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "isReady", false },
            { "selectedSkinId", -1 },
            { "finalSkinId", -1 }
        };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("ActivePlayer"))
        {
            activePlayer = (int)propertiesThatChanged["ActivePlayer"];
            ActionParams data = new ActionParams();
            data.Put("activePlayer", activePlayer);
            EventManager.TriggerEvent("ActivePlayer", data);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!VitoriaDerrota.instance.painelDerrota.activeInHierarchy && !VitoriaDerrota.instance.painelVitoria.activeInHierarchy)
        {
            if (PhotonNetwork.PlayerList.Length == 1)
            {
                var inputManager = FindAnyObjectByType<InputManager>();
                if (inputManager != null) inputManager.enabled = false;
                var turnManager = FindFirstObjectByType<TurnManager>();
                if (turnManager != null) turnManager.StopTimer();

                painelReconectar.SetActive(true);

                if (!PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
                }

                StartCoroutine(RedirectToLobbyAfterDelay(5f));
            }
        }
    }

    private IEnumerator RedirectToLobbyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        VoltarLobby();
        painelReconectar.SetActive(false);
    }


    public bool CheckForGameEnd(GridNodeS node)
    {
        //verifica se os reis foram capturados
        if (p1KingCaptured || p2KingCaptured)
        {
            return true;
        }

        if (node != null && node.x == p1GoalNode.x && node.y == p1GoalNode.y)
        {
            if (node.occupyingUnit != null && node.occupyingUnit.usType == UnitS.unitType.king &&
                node.occupyingUnit.ptype == UnitS.player.p1)
                return true;
        }
        if (node != null && node.x == p2GoalNode.x && node.y == p2GoalNode.y)
        {
            if (node.occupyingUnit != null && node.occupyingUnit.usType == UnitS.unitType.king &&
                node.occupyingUnit.ptype == UnitS.player.p2)
                return true;
        }

        return false;
    }

    public static void RemoveUnitAtNode(GridNodeS node)
    {
        if (node.occupyingUnit != null)
        {
            UnitS temp = node.occupyingUnit;
            if (temp.usType == UnitS.unitType.king)
            {
                if (temp.ptype == UnitS.player.p1)
                {
                    instance.p1KingCaptured = true;
                }
                else if (temp.ptype == UnitS.player.p2)
                {
                    instance.p2KingCaptured = true;
                }
            }

            RemoveUnitFromPlayerList(temp);
            node.occupyingUnit = null;
            temp.photonView.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }

    public static void RemoveUnitFromPlayerList(UnitS unit)
    {
        if (unit.ptype == UnitS.player.p1)
        {
            instance.player1Units.Remove(unit);
        }
        else
        {
            instance.player2Units.Remove(unit);
        }
    }

    public static List<GridNodeS> BuildUnitMoveList(OnitamaCard card, GridNodeS node)
    {
        List<CardMove> unitmoves = card.cardinfo.moveset;
        int x = node.x;
        int y = node.y;

        if (card.playerId == 1)
        {
            return instance.LegalUnitMoves(unitmoves, x, y, 1);
        }
        else
        {
            return instance.LegalUnitMoves(unitmoves, x, y, -1);
        }
    }

    private List<GridNodeS> LegalUnitMoves(List<CardMove> moves, int x, int y, int mult)
    {
        List<GridNodeS> rList = new List<GridNodeS>();

        foreach (CardMove move in moves)
        {
            int newX = move.x + x;
            int newY = move.y * mult + y;

            // Verifica se a nova posição está dentro do tabuleiro 5x5
            if (newX >= 0 && newX < GridManagerS.instance.dimensionX &&
                newY >= 0 && newY < GridManagerS.instance.dimensionY)
            {
                GridNodeS temp = GridManagerS.GetNodeS(newX, newY);
                if (temp != null)
                {
                    UnitS originUnit = GridManagerS.GetNodeS(x, y).occupyingUnit;
                    if (temp.occupyingUnit == null || temp.occupyingUnit.ptype != originUnit.ptype)
                    {
                        rList.Add(temp);
                    }
                }
            }
        }

        return rList;
    }

    public static List<UnitS> GetPlayerUnitList(int playerId)
    {
        return (playerId == 1) ? instance.player1Units : instance.player2Units;
    }

    public void CleanUpPlayerUnits()
    {
        foreach (UnitS unit in player1Units)
        {
            //unit.Destroy();
            unit.photonView.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
        foreach (UnitS unit in player2Units)
        {
            //unit.Destroy();
            unit.photonView.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }

        player1Units.Clear();
        player2Units.Clear();
    }

    public void RestartGame()
    {
        VitoriaDerrota.instance.painelDerrota.SetActive(false);
        VitoriaDerrota.instance.painelVitoria.SetActive(false);
        FindAnyObjectByType<InputManager>().enabled = true;
        FindFirstObjectByType<TurnManager>().RestartTimer();
        CardManager.RestartGame();
        instance.CleanUpPlayerUnits();
        instance.InitPlayers();

        instance.p1KingCaptured = false;
        instance.p2KingCaptured = false;

        //music teste
        MusicManager.instance.playMusicBattle();

        instance.RunGame();
    }

    //public void Sair()
    //{
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        PhotonNetwork.CurrentRoom.IsOpen = true;
    //        PhotonNetwork.CurrentRoom.IsVisible = true;
    //    }

    //    CreateAndJoin.instance.Voltar();
    //}

    public void Sair()
    {
        StartCoroutine(LeaveGameAndReturnToMenu());
    }

    private IEnumerator LeaveGameAndReturnToMenu()
    {

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();

            while (PhotonNetwork.InRoom)
            {
                yield return null;
            }
        }

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();

            while (PhotonNetwork.IsConnected)
            {
                yield return null;
            }
        }

        if (CreateAndJoin.instance != null)
        {
            Destroy(CreateAndJoin.instance.gameObject);
        }

        SceneManager.LoadScene("MenuPrincipal");
        MusicManager.instance.playMusicGeral();
    }

    public void VoltarLobby()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
    {
        { "isReady", false },
        { "selectedSkinId", -1 },
        { "finalSkinId", -1 }
    };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
        }

        CleanUpPlayerUnits();

        p1KingCaptured = false;
        p2KingCaptured = false;

        SceneManager.LoadScene("Lobby");
        MusicManager.instance.playMusicGeral();
    }

}
