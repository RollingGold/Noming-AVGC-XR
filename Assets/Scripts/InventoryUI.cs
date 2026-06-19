using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private InventorySlot[] slots;


    public void Refresh()
    {
        // Clear slots
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Clear();
        }

        // Fill slots
        for (int i = 0; i < inventory.items.Count; i++)
        {
            if (i >= slots.Length)
                break;

            slots[i].SetItem(
                inventory.items[i]
            );
        }
    }
}