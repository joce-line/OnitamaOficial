using Photon.Pun;
using TMPro;
using UnityEngine;

public class OnitamaCard : MonoBehaviourPun
{
    // TODO: #CartaLayout Atributos que a carta vai ter, revisar o layout da carta após a finalização do design
    public GameObject cardHolder;
    public Collider2D cardCollider;
    public TextMeshPro cardName;
    public SpriteRenderer cardBackgroundSprite;
    public SpriteRenderer cardMovesSprite;
    public SpriteRenderer cardDogSprite;
    public CardData cardinfo;
    public int playerId;

    [SerializeField]
    private SpriteRenderer highlightSprite;
    [SerializeField]
    private SpriteRenderer overlayShadeSprite;
    [SerializeField]
    private TextMeshPro upcomingText;

    [PunRPC]
    public void SetupCard(int cardIndex, int playerId)
    {

        CardData cardinfo2 = CardManager.GetInstance().cardSOList[cardIndex];
        if (cardinfo2 == null)
        {
            Debug.LogError($"CardData não encontrado para o índice: {cardIndex}");
            return;
        }

        this.cardinfo = cardinfo2;
        this.playerId = playerId;

        LoadCardData(cardinfo2);
        TurnOffHighlight();
        TurnOffUpcoming();

        if (playerId == 2)
        {
            Vector3 scale = cardHolder.transform.localScale;
            //scale.y *= -1; // Inverter para P2
            cardHolder.transform.localScale = scale;
        }

    }

    [PunRPC]
    public void MoveToPosition(Vector3 pos)
    {
        StartCoroutine(MoveUtils.SmoothLerp(1f, cardHolder.transform.position, pos, cardHolder, true));
    }
    public void LoadCardData(CardData cardinfo2)
    {
        if (cardName != null) cardName.text = cardinfo2.name;
        if (cardMovesSprite != null) cardMovesSprite.sprite = cardinfo2.gridSprite;
        if (cardDogSprite != null) cardDogSprite.sprite = cardinfo2.dogSprite;
    }

    [PunRPC]
    public void TurnOnHighlight()
    {
        //highlightSprite.enabled = true;
        if (highlightSprite != null && playerId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            highlightSprite.enabled = true;
        }
    }

    [PunRPC]
    public void TurnOffHighlight()
    {
        //highlightSprite.enabled = false;
        if (highlightSprite != null)
            highlightSprite.enabled = false;
    }

    //Upcoming é a proxima carta que vai vir, verificar prefab CardHolder para mudanças se necessario
    [PunRPC]
    public void TurnOnUpcoming()
    {
        //overlayShadeSprite.enabled = true;
        //upcomingText.enabled = true;
        //cardCollider.enabled = false;
        if (overlayShadeSprite != null)
            overlayShadeSprite.enabled = true;
        if (upcomingText != null)
            upcomingText.enabled = true;
        if (cardCollider != null)
            cardCollider.enabled = false;
    }

    [PunRPC]
    public void TurnOffUpcoming()
    {
        //overlayShadeSprite.enabled = false;
        //upcomingText.enabled = false;
        //cardCollider.enabled = true;
        if (overlayShadeSprite != null)
            overlayShadeSprite.enabled = false;
        if (upcomingText != null)
            upcomingText.enabled = false;
        if (cardCollider != null)
            cardCollider.enabled = true;
    }

    public void Destroy()
    {
        //Destroy(this.cardHolder);
        //Destroy(this);
        PhotonNetwork.Destroy(cardHolder);
    }
}
