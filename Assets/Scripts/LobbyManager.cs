using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Assets.scripts.InfoPlayer;
using System;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static LobbyManager instance;

    public Transform playerListParent;
    public GameObject statusPlayerPrefab;
    public Transform skinListParent;
    public GameObject skinChoicePrefab;
    public Button readyButton;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI roomNameText;

    private List<StatusPlayerItem> playerItems = new List<StatusPlayerItem>();
    private List<SkinChoiceItem> skinItems = new List<SkinChoiceItem>();
    private int selectedSkinId = -1;
    private bool isReady = false;
    private Dictionary<int, int> playerSkinChoices = new Dictionary<int, int>();

    public static LobbyManager GetInstance()
    {
        if (instance == null)
            instance = FindFirstObjectByType<LobbyManager>();
        return instance;
    }

    void Awake()
    {
        instance = this;
        if (GetComponent<PhotonView>() == null)
        {
            gameObject.AddComponent<PhotonView>();
        }
    }

    void Start()
    {
        isReady = false;
        selectedSkinId = -1;
        playerSkinChoices.Clear();

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
    {
        { "isReady", false },
        { "selectedSkinId", -1 },
        { "finalSkinId", -1 }
    };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        if (readyButton != null)
        {
            readyButton.interactable = false;
            readyButton.onClick.AddListener(OnReadyButtonClicked);
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        if (roomNameText != null)
        {
            roomNameText.text = $"Sala: {PhotonNetwork.CurrentRoom.Name}";
        }
        else
        {
            Debug.LogError("roomNameText não atribuído no LobbyManager!");
        }

        UpdatePlayerList();
        LoadPlayerSkins();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
        playerSkinChoices.Remove(otherPlayer.ActorNumber);
        UpdateSkinAvailability();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("isReady"))
        {
            UpdatePlayerStatus(targetPlayer);
            if (PhotonNetwork.IsMasterClient && AllPlayersReady() && FindFirstObjectByType<CreateAndJoin>().IsRoomFull())
            {
                photonView.RPC("StartCountdownRPC", RpcTarget.All);
            }
        }
        if (changedProps.ContainsKey("selectedSkinId"))
        {
            int skinId = (int)changedProps["selectedSkinId"];
            playerSkinChoices[targetPlayer.ActorNumber] = skinId;
            UpdateSkinAvailability();
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }
    }

    private void UpdatePlayerList()
    {
        foreach (var item in playerItems)
        {
            Destroy(item.gameObject);
        }
        playerItems.Clear();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            GameObject go = Instantiate(statusPlayerPrefab, playerListParent);
            StatusPlayerItem item = go.GetComponent<StatusPlayerItem>();
            string status = player.CustomProperties.ContainsKey("isReady") && (bool)player.CustomProperties["isReady"] ? "Pronto" : "Aguardando";
            item.ConfigurarPlayer(player.ActorNumber, player.NickName, status);
            playerItems.Add(item);
        }
    }

    private void UpdatePlayerStatus(Player player)
    {
        StatusPlayerItem item = playerItems.Find(x => x.GetActorNumber() == player.ActorNumber);
        if (item != null)
        {
            string status = player.CustomProperties.ContainsKey("isReady") && (bool)player.CustomProperties["isReady"] ? "Pronto" : "Aguardando";
            item.ConfigurarPlayer(player.ActorNumber, player.NickName, status);
        }
    }

    private void LoadPlayerSkins()
    {
        List<SkinData> skins = FetchSkinsFromDatabase(PlayerInfo.idPlayer);

        foreach (var skin in skins)
        {
            GameObject go = Instantiate(skinChoicePrefab, skinListParent);
            SkinChoiceItem item = go.GetComponent<SkinChoiceItem>();
            bool isSelectedByOther = playerSkinChoices.Any(x => x.Value == skin.id && x.Key != PhotonNetwork.LocalPlayer.ActorNumber);
            item.ConfigurarItem(skin.id, skin.nome, skin.caminhoPawn, skin.caminhoKing, isSelectedByOther);
            skinItems.Add(item);
        }
    }

    private List<SkinData> FetchSkinsFromDatabase(int playerId)
    {
        List<SkinData> skins = new List<SkinData>();

        string query = "SELECT cs.id_Conjunto, cs.nome_Conjunto, cs.caminho_Pawn, cs.caminho_King " +
                   "FROM skins_usuario su " +
                   "JOIN conjuntos_skins cs ON su.id_Conjunto = cs.id_Conjunto " +
                   $"WHERE su.id_Usuario = {playerId}";

        List<Dictionary<string, object>> results = DatabaseManager.Instance.ExecuteReader(query);

        foreach (var row in results)
        {
            try
            {
                skins.Add(new SkinData
                {
                    id = (int)row["id_Conjunto"],
                    nome = row["nome_Conjunto"].ToString(),
                    caminhoPawn = row["caminho_Pawn"].ToString(),
                    caminhoKing = row["caminho_King"].ToString()
                });
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erro ao processar skin: {e.Message}");
            }
        }

        if (skins.Count == 0)
        {
            Debug.LogWarning($"Nenhuma skin encontrada para o jogador {playerId}. Adicionando skin padrão.");
            skins.Add(new SkinData
            {
                id = 1,
                nome = "Default",
                caminhoPawn = "https://firebasestorage.googleapis.com/v0/b/ihc-manut.appspot.com/o/skins%2Fpawn_1747457311263_BrancoPeao.png?alt=media&token=8a5bade5-11e6-45be-80ac-b4963b5ebd67",
                caminhoKing = "https://firebasestorage.googleapis.com/v0/b/ihc-manut.appspot.com/o/skins%2Fking_1747457312118_BrancoRei.png?alt=media&token=f1045dd7-3361-4eb4-9973-8ec14944ac7e"
            });
        }

        return skins;
    }

    public void ConfirmarEscolha(int skinId, bool select)
    {
        if (select)
        {
            selectedSkinId = skinId;
            readyButton.interactable = true;
        }
        else if (selectedSkinId == skinId)
        {
            selectedSkinId = -1;
            readyButton.interactable = false;
        }
        else
        {
            return;
        }

        foreach (var item in skinItems)
        {
            item.SetSelected(item.ItemId == selectedSkinId);
        }

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "selectedSkinId", selectedSkinId }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    private void OnReadyButtonClicked()
    {
        if (selectedSkinId == -1)
        {
            Debug.LogWarning("Nenhuma skin selecionada!");
            return;
        }

        isReady = true;
        readyButton.interactable = false;

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "isReady", true },
            { "finalSkinId", selectedSkinId }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        PlayerInfo.SelectedSkinId = selectedSkinId;
    }

    private bool AllPlayersReady()
    {
        return PhotonNetwork.PlayerList.All(player => player.CustomProperties.ContainsKey("isReady") && (bool)player.CustomProperties["isReady"]);
    }

    [PunRPC]
    private void StartCountdownRPC()
    {
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = $"Iniciando em {i}...";
            yield return new WaitForSeconds(1f);
        }
        countdownText.text = "Iniciando!";
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }

    private void UpdateSkinAvailability()
    {
        foreach (var item in skinItems)
        {
            int itemId = item.ItemId;
            bool isSelectedByOther = playerSkinChoices.Any(x => x.Value == itemId && x.Key != PhotonNetwork.LocalPlayer.ActorNumber);
            bool isSelectedByMe = selectedSkinId == itemId;
            item.ConfigurarItem(itemId, item.nomeItem.text, item.CaminhoPawn, item.CaminhoKing, isSelectedByOther && !isSelectedByMe);
            item.SetSelected(isSelectedByMe);
        }
    }

    public void Voltar()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("LobbyTest");
    }
}
