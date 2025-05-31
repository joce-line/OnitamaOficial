using UnityEngine;

public class BackGround : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(Sprite sprite)
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = sprite;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        this.transform.position = new Vector3(0, 0, 1);
    }
}
