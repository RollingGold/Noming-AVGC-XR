using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopupUI : MonoBehaviour
{
    public static ItemPopupUI Instance;



    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;

    [Header("Scripts")]
    [SerializeField] private EquipmentManager equipmentManager;

    private ItemData currentItem;



    private void Awake()
    {
        Instance = this;


        Debug.Log(equipmentManager);

        gameObject.SetActive(false);
    }

    public void Show(ItemData item)
    {
        currentItem = item;

        itemIcon.sprite = item.icon;

        itemNameText.text =
            item.itemName;

        itemDescriptionText.text =
            item.description;

        gameObject.SetActive(true);
    }


    public void EquipItem()
    {
        if (currentItem == null)
            return;

        equipmentManager.Equip(currentItem);

        Close();
    }

    public void Close()
    {
        currentItem = null;

        gameObject.SetActive(false);
    }

    public ItemData GetCurrentItem()
    {
        return currentItem;
    }
}