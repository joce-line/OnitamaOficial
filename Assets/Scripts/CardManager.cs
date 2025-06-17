using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class CardManager : MonoBehaviourPunCallbacks
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

        // Inicializar propriedades sincronizadas
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "UpcomingCardViewID", -1 } });
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


        if (PhotonNetwork.IsMasterClient)
        {
            CreateCards();
        }
        EventManager.StartListening("EndPlayerMovement", OnEndPlayerMovement);
    }

    private Vector3 p1slot1 = new(-7.01f, -2, -5);
    private Vector3 p1slot2 = new(-3.86f, -2, -5);
    private Vector3 p1slot3 = new(7.28f, 0.31f, -5);
    private Vector3 p2slot1 = new(-7.01f, 2.36f, -5);
    private Vector3 p2slot2 = new(-3.86f, 2.36f, -5);
    private Vector3 p2slot3 = new(7.28f, 0.31f, -5);
    private List<Vector3> p1SlotList = new();
    private List<Vector3> p2SlotList = new();

    [PunRPC]
    public void CreateCardsRPC(int[] cardIndices, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (cardSOList == null || cardSOList.Count == 0)
        {
            Debug.LogError("cardSOList está vazia ou não foi inicializada!");
            return;
        }

        p1Cards.Clear();
        p2Cards.Clear();
        CreateCard(p1slot1, cardIndices[0], 1);
        CreateCard(p1slot2, cardIndices[1], 1);
        CreateCard(p1slot3, cardIndices[2], 1, true);
        CreateCard(p2slot1, cardIndices[3], 2);
        CreateCard(p2slot2, cardIndices[4], 2);

    }

    public static void CreateCards()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        List<int> rngCardPool = RngUtils.GetNonRepeatingNumberFromRange(0, 16, 5);
        instance.photonView.RPC("CreateCardsRPC", RpcTarget.AllBuffered, rngCardPool.ToArray());
    }

    public void CreateCard(Vector3 pos, int type, int playerId, bool upcoming = false)
    {
        if (type < 0 || type >= cardSOList.Count)
        {
            Debug.LogError($"Índice de carta inválido: {type}");
            return;
        }

        GameObject temp = PhotonNetwork.Instantiate(cardPrefab.name, pos, Quaternion.identity);
        OnitamaCard temp2 = temp.GetComponentInChildren<OnitamaCard>();

        temp2.photonView.RPC("SetupCard", RpcTarget.AllBuffered, type, playerId);

        if (upcoming)
        {
            upcomingCard = temp2;
            temp2.photonView.RPC("TurnOnUpcoming", RpcTarget.AllBuffered);
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "UpcomingCardViewID", temp2.photonView.ViewID } });
        }

        if (playerId == 1)
        {
            p1Cards.Add(temp2);
        }
        else
        {
            p2Cards.Add(temp2);
        }
    }

    public static OnitamaCard SelectRandomPlayerCard(int playerId)
    {
        List<OnitamaCard> cards = playerId == 1 ? instance.p1Cards : instance.p2Cards;

        cards = cards.Where(card => card != instance.upcomingCard).ToList();
        
        if (cards.Count == 0)
        {
            Debug.LogError($"Nenhuma carta disponível para o jogador {playerId}");
            return null;
        }
        int randCard = Random.Range(0, cards.Count);
        return cards[randCard];

    }

    public void OnEndPlayerMovement(string eventName, ActionParams data)
    {

        int receivedPlayer = data.Get<int>("activePlayer");
        OnitamaCard receivedCard = data.Get<OnitamaCard>("lastSelectedCard");
        upcomingCard.TurnOffUpcoming();

        int[] p1CardViewIDs = p1Cards.Select(card => card.photonView.ViewID).ToArray();
        int[] p2CardViewIDs = p2Cards.Select(card => card.photonView.ViewID).ToArray();
        photonView.RPC("SyncEndPlayerMovement", RpcTarget.AllBuffered, receivedPlayer, receivedCard.photonView.ViewID, p1CardViewIDs, p2CardViewIDs);
    }

    [PunRPC]
    public void SyncEndPlayerMovement(int receivedPlayer, int receivedCardViewID, int[] p1CardViewIDs, int[] p2CardViewIDs)
    {
        OnitamaCard receivedCard = PhotonView.Find(receivedCardViewID)?.GetComponent<OnitamaCard>();
        
        receivedCard.photonView.RPC("TurnOffHighlight", RpcTarget.All);

        if (upcomingCard != null)
        {
            upcomingCard.photonView.RPC("TurnOffUpcoming", RpcTarget.All);
        }

        p1Cards.Clear();
        foreach (int viewID in p1CardViewIDs)
        {
            OnitamaCard card = PhotonView.Find(viewID)?.GetComponent<OnitamaCard>();
            if (card != null)
            {
                p1Cards.Add(card);
            }
        }

        p2Cards.Clear();
        foreach (int viewID in p2CardViewIDs)
        {
            OnitamaCard card = PhotonView.Find(viewID)?.GetComponent<OnitamaCard>();
            if (card != null)
            {
                p2Cards.Add(card);
            }
        }

        if (receivedPlayer == 1)
        {
            if (upcomingCard != null && upcomingCard.playerId == 2)
            {
                p2Cards.Remove(upcomingCard);
                p1Cards.Add(upcomingCard);
                upcomingCard.photonView.RPC("SetupCard", RpcTarget.AllBuffered, cardSOList.IndexOf(upcomingCard.cardinfo), 1);
                MoveCardToPosition(p1SlotList[p1Cards.Count - 1], upcomingCard);
            }

            for (int i = 0; i < p1Cards.Count; i++)
            {
                if (p1Cards[i] == receivedCard)
                {
                    p1Cards.RemoveAt(i);
                    p2Cards.Add(receivedCard);
                    receivedCard.photonView.RPC("SetupCard", RpcTarget.AllBuffered, cardSOList.IndexOf(receivedCard.cardinfo), 2);
                    MoveCardToPosition(p2slot3, receivedCard);
                    receivedCard.photonView.RPC("ResetCardRotation", RpcTarget.AllBuffered);
                    receivedCard.photonView.RPC("TurnOnUpcoming", RpcTarget.All);
                    upcomingCard = receivedCard;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "UpcomingCardViewID", upcomingCard.photonView.ViewID } });
                    break;
                }
            }

            for (int i = 0; i < p1Cards.Count && i < p1SlotList.Count; i++)
            {
                MoveCardToPosition(p1SlotList[i], p1Cards[i]);
            }

            for (int i = 0; i < p2Cards.Count; i++)
            {
                if (!p2Cards[i].photonView.IsMine)
                {
                    p2Cards[i].photonView.RPC("RotateCardForP2", RpcTarget.AllBuffered);
                }
            }
        }
        else
        {
            if (upcomingCard != null && upcomingCard.playerId == 1)
            {
                p1Cards.Remove(upcomingCard);
                p2Cards.Add(upcomingCard);
                upcomingCard.photonView.RPC("SetupCard", RpcTarget.AllBuffered, cardSOList.IndexOf(upcomingCard.cardinfo), 2);
                MoveCardToPosition(p2SlotList[p2Cards.Count - 1], upcomingCard);
                upcomingCard.photonView.RPC("RotateCardForP2", RpcTarget.AllBuffered);
            }

            for (int i = 0; i < p2Cards.Count; i++)
            {
                if (p2Cards[i] == receivedCard)
                {
                    p2Cards.RemoveAt(i);
                    p1Cards.Add(receivedCard);
                    receivedCard.photonView.RPC("SetupCard", RpcTarget.AllBuffered, cardSOList.IndexOf(receivedCard.cardinfo), 1);
                    MoveCardToPosition(p1slot3, receivedCard);
                    receivedCard.photonView.RPC("ResetCardRotation", RpcTarget.AllBuffered);
                    receivedCard.photonView.RPC("TurnOnUpcoming", RpcTarget.All);
                    upcomingCard = receivedCard;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "UpcomingCardViewID", upcomingCard.photonView.ViewID } });
                    break;
                }
            }

            for (int i = 0; i < p2Cards.Count && i < p2SlotList.Count; i++)
            {
                MoveCardToPosition(p2SlotList[i], p2Cards[i]);
                if (!p2Cards[i].photonView.IsMine)
                {
                    p2Cards[i].photonView.RPC("RotateCardForP2", RpcTarget.AllBuffered);
                }
            }
        }

    }
    private void MoveCardToPosition(Vector3 pos, OnitamaCard card)
    {
        card.photonView.RPC("MoveToPosition", RpcTarget.AllBuffered, pos);

    }

    [PunRPC]
    private void MoveToPosition(Vector3 pos)
    {
        StartCoroutine(MoveUtils.SmoothLerp(1f, cardPrefab.transform.position, pos, cardPrefab, true));
    }

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