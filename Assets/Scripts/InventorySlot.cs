using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot :
    MonoBehaviour,
    IPointerClickHandler
{
    [SerializeField] private Image itemIcon;

    private ItemData currentItem;

    public void SetItem(ItemData item)
    {
        currentItem = item;

        itemIcon.sprite = item.icon;
        itemIcon.enabled = true;
    }

    public void Clear()
    {
        currentItem = null;

        itemIcon.sprite = null;
        itemIcon.enabled = false;
    }

    public void OnPointerClick(
    PointerEventData eventData)
    {
        if (currentItem == null)
            return;

        ItemPopupUI.Instance.Show(
            currentItem
        );
    }
}