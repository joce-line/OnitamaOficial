using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public int activePlayer = 0;
    private float indicatorTimer = 10f;
    private float maxIndicatorTimer = 10f;
    private bool isGameOver = false;
    public int nextPlayer;
    private float syncTimer = 0f;
    private float syncInterval = 0.1f;

    [SerializeField] private Image radialIndicatorUI = null;

    void Start()
    {
        EventManager.StartListening("GameOver", OnGameOver);
        EventManager.StartListening("EndPlayerMovement", OnEndPlayerMovement);

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

            if (maxIndicatorTimer <= 0f)
            {
                maxIndicatorTimer = indicatorTimer;
                activePlayer = (activePlayer == 1) ? 2 : 1;
                TriggerActivePlayerEvent(activePlayer);
                photonView.RPC("RPC_SyncTimer", RpcTarget.All, maxIndicatorTimer);
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
    private void OnGameOver(string eventName, ActionParams data)
    {
        string gameOver = data.Get<string>("GameOver");
        Debug.Log($"isGameOver = {gameOver}");
        isGameOver = true;
        radialIndicatorUI.enabled = false;
    }

    private void onActivePlayerChanged(string eventName, ActionParams data)
    {
        activePlayer = data.Get<int>("activePlayer");
    }

    private void OnEndPlayerMovement(string eventName, ActionParams data)
    {
        int receivedPlayer = data.Get<int>("activePlayer");
        maxIndicatorTimer = indicatorTimer;
        photonView.RPC("RPC_SyncTimer", RpcTarget.All, maxIndicatorTimer);
        activePlayer = (activePlayer == 1) ? 2 : 1;
        TriggerActivePlayerEvent(activePlayer);

    }
    public void RestartTimer()
    {
        maxIndicatorTimer = indicatorTimer;
        isGameOver = false;
        radialIndicatorUI.enabled = true;
        //photonView.RPC("RPC_SyncTimer", RpcTarget.All, maxIndicatorTimer); Talvez mudar quando colocar no botao.
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_SyncTimer", RpcTarget.All, maxIndicatorTimer);
        }
    }
}