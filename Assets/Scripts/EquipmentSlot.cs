using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    [SerializeField] private ItemType acceptedType;
    [SerializeField] private Image itemIcon;

    private ItemData equippedItem;

    public bool CanEquip(
        ItemData item)
    {
        return item.itemType ==
            acceptedType;
    }

    public ItemData Equip(ItemData item)
    {
        if (!CanEquip(item))
            return null;

        ItemData previousItem =
            equippedItem;

        equippedItem = item;

        itemIcon.sprite =
            item.icon;

        itemIcon.enabled = true;

        return previousItem;
    }

    public void Unequip()
    {
        equippedItem = null;

        itemIcon.sprite = null;
        itemIcon.enabled = false;
    }

    public ItemData GetEquippedItem()
    {
        return equippedItem;
    }

    public bool IsEmpty()
    {
        return equippedItem == null;
    }
}