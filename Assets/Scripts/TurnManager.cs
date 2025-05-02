using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public int activePlayer = 0;
    private float indicatorTimer = 10f;
    private float maxIndicatorTimer = 10f;
    private bool isGameOver = false;
    public int nextPlayer;

    [SerializeField] private Image radialIndicatorUI = null;

    void Start()
    {
        EventManager.StartListening("GameOver", OnGameOver);
        EventManager.StartListening("ActivePlayer", onActivePlayerChanged);
        EventManager.StartListening("EndPlayerMovement", OnEndPlayerMovement);
    }

    void Update()
    {
        if (!isGameOver)
        {
            maxIndicatorTimer -= Time.deltaTime;
            radialIndicatorUI.enabled = true;
            radialIndicatorUI.fillAmount = maxIndicatorTimer / indicatorTimer;

            if (maxIndicatorTimer <= 0)
            {
                maxIndicatorTimer = indicatorTimer;
                activePlayer = (activePlayer == 1) ? 2 : 1;
                TurnChange();
            }
        }

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
}