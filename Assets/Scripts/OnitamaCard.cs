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
            //Vector3 scale = cardHolder.transform.localScale;
            ////scale.y *= -1; // Inverter para P2
            //cardHolder.transform.localScale = scale;
            photonView.RPC("RotateCardForP2", RpcTarget.AllBuffered);
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
        if (highlightSprite != null && playerId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            highlightSprite.enabled = true;
        }
    }

    [PunRPC]
    public void TurnOffHighlight()
    {
        if (highlightSprite != null)
            highlightSprite.enabled = false;
    }

    //Upcoming é a proxima carta que vai vir, verificar prefab CardHolder para mudanças se necessario
    [PunRPC]
    public void TurnOnUpcoming()
    {
        if (overlayShadeSprite != null)
            overlayShadeSprite.enabled = true;
        if (upcomingText != null)
            upcomingText.enabled = true;
        if (cardCollider != null)
            cardCollider.enabled = false;

        if (playerId == 2)
        {
            photonView.RPC("ResetCardRotation", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void TurnOffUpcoming()
    {
        if (overlayShadeSprite != null)
            overlayShadeSprite.enabled = false;
        if (upcomingText != null)
            upcomingText.enabled = false;
        if (cardCollider != null)
            cardCollider.enabled = true;
    }

    [PunRPC]
    public void RotateCardForP2()
    {
        if (!overlayShadeSprite.enabled)
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, 180);
            StartCoroutine(MoveUtils.SmoothRotate(1f, targetRotation, cardHolder));
        }
    }

    [PunRPC]
    public void ResetCardRotation()
    {
        Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
        StartCoroutine(MoveUtils.SmoothRotate(1f, targetRotation, cardHolder));
    }

    public void Destroy()
    {
        PhotonNetwork.Destroy(cardHolder);
    }
}
