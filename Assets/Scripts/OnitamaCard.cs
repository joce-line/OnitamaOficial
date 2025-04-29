using TMPro;
using UnityEngine;

public class OnitamaCard : MonoBehaviour
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

    void Start()
    {
        cardinfo = cardinfo.GetInstance();
        cardName.text = cardinfo.name;
        cardMovesSprite.sprite = cardinfo.gridSprite;
        cardDogSprite.sprite = cardinfo.dogSprite;
    }

    //coloca na carta as informações corretas de nome e movimentação (sprite)
    public void LoadCardData(CardData cardinfo2)
    {
        cardName.text = cardinfo2.name;
        cardMovesSprite.sprite = cardinfo2.gridSprite;
        cardDogSprite.sprite = cardinfo2.dogSprite;
    }

    public void TurnOnHighlight() => highlightSprite.enabled = true;

    public void TurnOffHighlight() => highlightSprite.enabled = false;

    //Upcoming é a proxima carta que vai vir, verificar prefab CardHolder para mudanças se necessario
    public void TurnOnUpcoming()
    {
        overlayShadeSprite.enabled = true;
        upcomingText.enabled = true;
        cardCollider.enabled = false;
    }

    public void TurnOffUpcoming()
    {
        overlayShadeSprite.enabled = false;
        upcomingText.enabled = false;
        cardCollider.enabled = true;
    }

    public void Destroy()
    {
        Destroy(this.cardHolder);
        Destroy(this);
    }
}
