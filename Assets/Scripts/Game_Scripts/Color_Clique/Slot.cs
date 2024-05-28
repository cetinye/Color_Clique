using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public Sprite GetSprite()
    {
        return spriteRenderer.sprite;
    }
}
