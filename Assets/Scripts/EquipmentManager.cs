using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [Header("Starter Equipments")]
    [SerializeField] private ItemData startingWeapon;

    //[Header("Scripts")]
    //[SerializeField] private Inventory inventory;

    [Header("Equipments Slots")]
    [SerializeField] private EquipmentSlot[] slots;

    private Inventory inventory;

    private void Start()
    {
        
        inventory = GetComponent<Inventory>();

        Equip(startingWeapon);
    }


    public void Equip(ItemData item)
    {
        foreach (
            EquipmentSlot slot
            in slots)
        {
            if (!slot.CanEquip(item))
                continue;

            ItemData previousItem =
                slot.Equip(item);

            if (previousItem != null)
            {
                inventory.AddItem(
                    previousItem
                );
            }

            inventory.RemoveItem(
                item
            );

            return;
        }
    }

    public EquipmentSlot[] GetSlots()
    {
        return slots;
    }
}