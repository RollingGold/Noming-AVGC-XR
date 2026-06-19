using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    [SerializeField]
    private ItemData[] items;

    private void Awake()
    {
        Instance = this;
    }

    public ItemData GetItem(string id)
    {
        foreach (ItemData item in items)
        {
            if (item.itemID == id)
                return item;
        }

        return null;
    }
}