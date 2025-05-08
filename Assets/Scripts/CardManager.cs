using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public List<CardData> cardSOList = new();
    public List<OnitamaCard> p1Cards = new();
    public List<OnitamaCard> p2Cards = new();

    public OnitamaCard upcomingCard;
    public static CardManager instance;

    public static List<OnitamaCard> GetP2Cards()
    {
        return instance.p2Cards;
    }

    public static CardManager GetInstance()
    {
        if (instance == null)
        {
            instance = (CardManager)FindFirstObjectByType(typeof(CardManager));
        }
        return instance;
    }

    public void Awake()
    {
        if (instance == null)
        {
            instance = (CardManager)FindFirstObjectByType(typeof(CardManager));
            if (instance == null)
            {
                instance = this;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        p1SlotList.Add(p1slot1);
        p1SlotList.Add(p1slot2);
        p1SlotList.Add(p1slot3);
        p2SlotList.Add(p2slot1);
        p2SlotList.Add(p2slot2);
        p2SlotList.Add(p2slot3);


        CreateCards();
        EventManager.StartListening("EndPlayerMovement", OnEndPlayerMovement);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // TODO: posição das cartas que vão aparecer, apos design verificar
    private Vector3 p1slot1 = new(-6.5f, -2, -5);
    private Vector3 p1slot2 = new(-4, -2, -5);
    private Vector3 p1slot3 = new(4, 0, -5);
    private Vector3 p2slot1 = new(-6.5f, 2, -5);
    private Vector3 p2slot2 = new(-4, 2, -5);
    private Vector3 p2slot3 = new(4, 0, -5);
    private List<Vector3> p1SlotList = new();
    private List<Vector3> p2SlotList = new();

    public static void CreateCards()
    {
        List<int> rngCardPool = RngUtils.GetNonRepeatingNumberFromRange(0, 16, 5);
        instance.CreateCard(instance.p1slot1, rngCardPool[0], 1);
        instance.CreateCard(instance.p1slot2, rngCardPool[1], 1);
        instance.CreateCard(instance.p1slot3, rngCardPool[2], 1, true);

        instance.CreateCard(instance.p2slot1, rngCardPool[3], 2);
        instance.CreateCard(instance.p2slot2, rngCardPool[4], 2);
        //instance.CreateCard(new Vector3(11,.5f,8),rngCardPool[5],2);
    }

    public void CreateCard(Vector3 pos, int type, int playerId, bool upcoming = false)
    {
        GameObject temp = Instantiate(cardPrefab, pos, Quaternion.identity);
        OnitamaCard temp2 = temp.GetComponentInChildren<OnitamaCard>();
        temp2.cardinfo = cardSOList[type];
        temp2.playerId = playerId;
        temp2.LoadCardData(cardSOList[type]);
        temp2.TurnOffHighlight();
        temp2.TurnOffUpcoming();

        if (upcoming)
        {
            upcomingCard = temp2;
            upcomingCard.TurnOnUpcoming();
        }

        if (playerId == 1)
        {
            p1Cards.Add(temp2);
        }
        else
        {            
            Vector3 scale = temp2.cardHolder.transform.localScale;
            //scale.y *= -1; //invertida para o p2
            temp2.cardHolder.transform.localScale = scale;
            p2Cards.Add(temp2);
        }
    }

    public static OnitamaCard SelectRandomPlayerCard(int playerId)
    {
        //tem apenas 2 cartas para um jogador escolher
        int randCard = Random.Range(0, 2);

        if (playerId == 1)
        {
            return instance.p1Cards[randCard];
        }
        else
        {
            return instance.p2Cards[randCard];
        }
    }

    public void OnEndPlayerMovement(string eventName, ActionParams data)
    {
        int receivedPlayer = data.Get<int>("activePlayer");
        OnitamaCard receivedCard = data.Get<OnitamaCard>("lastSelectedCard");
        upcomingCard.TurnOffUpcoming();
        if (receivedPlayer == 1)
        {
            if (upcomingCard != null && upcomingCard.playerId == 2)
            {

                p2Cards.Remove(upcomingCard);
                p1Cards.Add(upcomingCard);
                MoveCardToPosition(p1slot3, upcomingCard);
                upcomingCard.TurnOffUpcoming();
            }

            for (int i = 0; i < p1Cards.Count; i++)
            {
                if (p1Cards[i] == receivedCard)
                {
                    p1Cards[i].playerId = 2;
                    p2Cards.Add(p1Cards[i]);
                    //RotateCard(p1Cards[i]); //Rotação para mostrar ao outro jogador
                    MoveCardToPosition(p2slot3, p1Cards[i]);
                    p1Cards[i].TurnOnUpcoming();
                    upcomingCard = p1Cards[i];
                    p1Cards.RemoveAt(i);
                    break;
                }
            }

            for (int i = 0; i < p1Cards.Count; i++)
            {
                MoveCardToPosition(p1SlotList[i], p1Cards[i]);
            }
        }
        else
        {
            if (upcomingCard != null && upcomingCard.playerId == 1)
            {
                p1Cards.Remove(upcomingCard);
                p2Cards.Add(upcomingCard);
                MoveCardToPosition(p2slot3, upcomingCard);
                upcomingCard.TurnOffUpcoming();
            }

            for (int i = 0; i < p2Cards.Count; i++)
            {
                if (p2Cards[i] == receivedCard)
                {
                    p2Cards[i].playerId = 1;
                    p1Cards.Add(p2Cards[i]);
                    //RotateCard(p2Cards[i]);
                    MoveCardToPosition(p1slot3, p2Cards[i]);
                    p2Cards[i].TurnOnUpcoming();
                    upcomingCard = p2Cards[i];
                    p2Cards.RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < p2Cards.Count; i++)
            {
                MoveCardToPosition(p2SlotList[i], p2Cards[i]);
            }

        }
    }
        
    private void MoveCardToPosition(Vector3 pos, OnitamaCard card)
    {
        StartCoroutine(MoveUtils.SmoothLerp(1f, card.cardHolder.transform.position, pos, card.cardHolder, true));

    }

    // TODO: Movimentação das rotação de cartas, verificar se vai usar apos design pronto
    public void RotateCard(OnitamaCard card)
    {
        Quaternion targetRotation = card.cardHolder.transform.rotation * Quaternion.Euler(0, 0, 180);
        StartCoroutine(MoveUtils.SmoothRotate(2f, targetRotation, card.cardHolder));
    }

    public void CleanUpCards()
    {
        foreach (OnitamaCard card in p1Cards)
        {
            card.Destroy();
        }
        foreach (OnitamaCard card in p2Cards)
        {
            card.Destroy();
        }
        p1Cards.Clear();
        p2Cards.Clear();
    }

    public static void RestartGame()
    {
        instance.CleanUpCards();
        CreateCards();
    }
}
