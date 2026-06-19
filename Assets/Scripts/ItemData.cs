using UnityEngine;

[CreateAssetMenu(
    fileName = "New Item",
    menuName = "Inventory/Item"
)]
public class ItemData : ScriptableObject
{
    public string itemID;

    public string itemName;

    public Sprite icon;

    public ItemType itemType;

    [TextArea]
    public string description;
}

public enum ItemType
{
    Weapon,
    Helmet,
    Chest,
    Leggings,
    Boots,
    Ring,
    Additional,
    Consumable

}
