using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer slotRenderer;
    [SerializeField] private SpriteRenderer itemRenderer;

    public void SetSlotColor(Color newColor)
    {
        slotRenderer.color = newColor;
    }

    public Color GetSlotColor()
    {
        return slotRenderer.color;
    }

    public void SetItemSprite(Sprite sprite)
    {
        itemRenderer.sprite = sprite;
    }

    public Sprite GetItemSprite()
    {
        return itemRenderer.sprite;
    }
}
