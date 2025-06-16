using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public int activePlayer = 0;
    private float indicatorTimer = 50f;
    private float maxIndicatorTimer = 50f;
    private bool isGameOver = false;
    private bool isTimeoutProcessing = false;
    public int nextPlayer;
    private float syncTimer = 0f;
    private float syncInterval = 0.5f;

    [SerializeField] private Image radialIndicatorUI = null;

    void Start()
    {
        EventManager.StartListening("GameOver", OnGameOver);
        EventManager.StartListening("EndPlayerMovement", OnEndPlayerMovement);
        EventManager.StartListening("ActivePlayer", OnActivePlayerChange);

        if (PhotonNetwork.IsMasterClient)
        {

            activePlayer = 1;
            maxIndicatorTimer = indicatorTimer;
            TriggerActivePlayerEvent(activePlayer);
            photonView.RPC("RPC_SyncTimer", RpcTarget.All, maxIndicatorTimer);
        }

    }

    void Update()
    {
        if (!isGameOver)
        {
            radialIndicatorUI.enabled = true;
            radialIndicatorUI.fillAmount = maxIndicatorTimer / indicatorTimer;
        }

        if (!isGameOver && PhotonNetwork.IsMasterClient)
        {
            maxIndicatorTimer -= Time.deltaTime;

            syncTimer += Time.deltaTime;
            if (syncTimer >= syncInterval)
            {
                photonView.RPC("RPC_SyncTimer", RpcTarget.Others, maxIndicatorTimer);
                syncTimer = 0f;
            }

            if (maxIndicatorTimer <= 0f && !isTimeoutProcessing)
            {
                isTimeoutProcessing = true;
                maxIndicatorTimer = indicatorTimer;
                photonView.RPC("RPC_SyncTimer", RpcTarget.All, maxIndicatorTimer);
                photonView.RPC("RPC_TimeoutPlayerChange", RpcTarget.MasterClient);
                photonView.RPC("RPC_ClearNodeHighlights", RpcTarget.All);
            }

        }

    }
    private void TriggerActivePlayerEvent(int newActivePlayer)
    {
        photonView.RPC("RPC_TriggerActivePlayer", RpcTarget.All, newActivePlayer);
    }

    [PunRPC]
    void RPC_TriggerActivePlayer(int newActivePlayer)
    {
        activePlayer = newActivePlayer;
        ActionParams data = new ActionParams();
        data.Put("activePlayer", newActivePlayer);
        EventManager.TriggerEvent("ActivePlayer", data);
    }

    [PunRPC]
    void RPC_SyncTimer(float syncedTime)
    {
        maxIndicatorTimer = syncedTime;
        radialIndicatorUI.fillAmount = maxIndicatorTimer / indicatorTimer;
        radialIndicatorUI.enabled = true;
    }

    [PunRPC]
    void RPC_TimeoutPlayerChange()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        ActionParams data = new ActionParams();
        data.Put("activePlayer", activePlayer);
        EventManager.TriggerEvent("TimeoutPlayerChange", data);
        isTimeoutProcessing = false;
    }

    [PunRPC]
    void RPC_ClearNodeHighlights()
    {
        GridManagerS.GetInstance().ClearNodeHighlights();
    }
    private void OnGameOver(string eventName, ActionParams data)
    {
        string gameOver = data.Get<string>("GameOver");
        Debug.Log($"isGameOver = {gameOver}");
        isGameOver = true;
        radialIndicatorUI.enabled = false;
    }
    private void OnEndPlayerMovement(string eventName, ActionParams data)
    {
        //int receivedPlayer = data.Get<int>("activePlayer");
        //maxIndicatorTimer = indicatorTimer;
        //photonView.RPC("RPC_SyncTimer", RpcTarget.All, maxIndicatorTimer);
        maxIndicatorTimer = indicatorTimer;
        photonView.RPC("RPC_SyncTimer", RpcTarget.All, maxIndicatorTimer);
        photonView.RPC("RPC_TimeoutPlayerChange", RpcTarget.MasterClient);

    }

    private void OnActivePlayerChange(string eventName, ActionParams data)
    {
        activePlayer = data.Get<int>("activePlayer");
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("ActivePlayer"))
        {
            activePlayer = (int)propertiesThatChanged["ActivePlayer"];
        }
    }
    public void RestartTimer()
    {
        maxIndicatorTimer = indicatorTimer;
        isGameOver = false;
        radialIndicatorUI.enabled = true;
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_SyncTimer", RpcTarget.All, maxIndicatorTimer);
        }
    }

    public void StopTimer()
    {
        isGameOver = true;
        radialIndicatorUI.enabled = false;
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_StopTimer", RpcTarget.All);
        }
    }

    [PunRPC]
    void RPC_StopTimer()
    {
        isGameOver = true;
        radialIndicatorUI.enabled = false;
    }
}