using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public int Price;
    public string Name;
    public Sprite Sprite;

    public void SetIcon(Sprite icon)
    {
        if (Sprite == null)
        {
            Sprite = icon;
        }
    }
}
