using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.scripts.InfoPlayer;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public List<UnitS> player1Units = new List<UnitS>();
    public List<UnitS> player2Units = new List<UnitS>();
    public GameObject playerPawnPrefab;
    public GameObject playerKingPrefab;

    public bool pturn = true;
    public int turnNum = -1;
    private bool p1KingCaptured = false;
    private bool p2KingCaptured = false;
    private GridNodeS p1GoalNode;
    private GridNodeS p2GoalNode;
    private GameObject goal1Tmp;

    public GameObject GoalText;

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

        InitPlayers();
        InitGoalNodes();
        EventManager.StartListening("EndPlayerMovement", OnEndPlayerMovement);
        RunGame();
    }

    public void InitPlayers()
    {
        Debug.Log($"InitPlayers executado por {PhotonNetwork.NickName} (ActorNumber: {PhotonNetwork.LocalPlayer.ActorNumber})");
        Debug.Log("InitPlayers chamado por: " + PhotonNetwork.NickName + " | IsMasterClient: " + PhotonNetwork.IsMasterClient);

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
        ActionParams data = new ActionParams();
        data.Put("activePlayer", activePlayer);
        EventManager.TriggerEvent("ActivePlayer", data);
    }
    public void CreateUnit(int playerId, int x, int y, UnitS.unitType utype)
    {
        string prefabName = (utype == UnitS.unitType.pawn) ? "Prefabs/PlayerPawn" : "Prefabs/PlayerKing";
        GameObject newGo = PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity);

        // spritePath para usar no Load
        string spritePath = (playerId == 1)
            ? (utype == UnitS.unitType.pawn ? "Skins/PretoPeao" : "Skins/PretoRei")
            : (utype == UnitS.unitType.pawn ? "Skins/BrancoPeao" : "Skins/BrancoRei");

        int unitId = newGo.GetComponent<PhotonView>().ViewID;

        newGo.GetComponent<PhotonView>().RPC(
            "RPC_SetData",
            RpcTarget.AllBuffered,
            unitId,
            x,
            y,
            spritePath
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
            DadosJogo.vencedor = receivedPlayer;
            DadosJogo.perdedor = (receivedPlayer == 1) ? 2 : 1;


            ActionParams temp = new ActionParams();
            temp.Put("activePlayer", activePlayer);
            EventManager.TriggerEvent("GameOver", temp);


            VitoriaDerrota.instance.FimDeJogo();
            FindAnyObjectByType<InputManager>().enabled = false;

        }
        else
        {
            activePlayer = (receivedPlayer == 1) ? 2 : 1;


            ActionParams temp = new ActionParams();
            temp.Put("activePlayer", activePlayer);
            EventManager.TriggerEvent("ActivePlayer", temp);

        }
    }

    public bool CheckForGameEnd(GridNodeS node)
    {
        //verifica se os reis foram capturados
        if (p1KingCaptured || p2KingCaptured)
        {
            return true;
        }

        //Verifica se o rei adversario chegou no campo do outro
        if (node == p1GoalNode)
        {
            if (p1GoalNode.occupyingUnit.usType == UnitS.unitType.king &&
                p1GoalNode.occupyingUnit.ptype == UnitS.player.p1)
            {
                return true;
            }
        }
        if (node == p2GoalNode)
        {
            if (p2GoalNode.occupyingUnit.usType == UnitS.unitType.king &&
                p2GoalNode.occupyingUnit.ptype == UnitS.player.p2)
            {
                return true;
            }
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
                if (temp != null) // Mantém a robustez
                {
                    //rList.Add(temp);
                    // if (temp.occupyingUnit == null ||
                    //temp.occupyingUnit.ptype != GridManagerS.GetNodeS(x, y).occupyingUnit.ptype)
                    // {
                    //     rList.Add(temp);
                    // }

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
        instance.RunGame();
    }

    public void Sair()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
