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
        if (!PhotonNetwork.IsMasterClient)
        {
            this.enabled = false;
            return;
        }

        
        EventManager.StartListening("GameOver", OnGameOver);
        EventManager.StartListening("EndPlayerMovement", OnEndPlayerMovement);
        //EventManager.StartListening("ActivePlayer", onActivePlayerChanged);
        activePlayer = 1;
        TriggerActivePlayerEvent(activePlayer);
    }

    void Update()
    {
        if (!isGameOver && PhotonNetwork.IsMasterClient)
        {
            maxIndicatorTimer -= Time.deltaTime;
            radialIndicatorUI.enabled = true;
            radialIndicatorUI.fillAmount = maxIndicatorTimer / indicatorTimer;

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
                //TurnChange();
                TriggerActivePlayerEvent(activePlayer);
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
        maxIndicatorTimer = indicatorTimer;
    }

    private void TurnChange()
    {
        ActionParams data = new ActionParams();
        OnitamaCard receivedCard = data.Get<OnitamaCard>("lastSelectedCard");
        data.Put("activePlayer", activePlayer);
        EventManager.TriggerEvent("ActivePlayer", data);

    }
    public void RestartTimer()
    {
        maxIndicatorTimer = indicatorTimer;
        isGameOver = false;
        radialIndicatorUI.enabled = true;
    }
}