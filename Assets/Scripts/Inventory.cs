using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Save System")]
    [SerializeField] private SaveManager saveManager;

    [Header("UI")]
    [SerializeField] private InventoryUI inventoryUI;

    public List<ItemData> items =
        new List<ItemData>();

    public void AddItem(ItemData item)
    {
        items.Add(item);

        inventoryUI.Refresh();

        Debug.Log(
            "Picked up " +
            item.itemName
        );

        saveManager.AutoSave();
    }

    public void RemoveItem(ItemData item)
    {
        items.Remove(item);

        inventoryUI.Refresh();

        saveManager.AutoSave();
    }
}