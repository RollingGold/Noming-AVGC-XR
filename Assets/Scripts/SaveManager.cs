using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    [Header("Sceneloader")]
    [SerializeField] private SceneLoader sceneLoader;

    [Header("Inventory")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private ItemDatabase itemDatabase;

    public static bool LoadGame;

    public static bool CanAutoSave = true;

    public static SaveManager Instance;

    private string savePath;

    private void Awake()
    {
        if (sceneLoader == null)
        {
            sceneLoader = GetComponent<SceneLoader>();
        }
        if (inventory == null)
        {
            inventory = GetComponent<Inventory>();
        }
        if (inventoryUI == null)
        {
            inventoryUI = GetComponent<InventoryUI>();
        }
        if (equipmentManager == null)
        {
            equipmentManager = GetComponent<EquipmentManager>();
        }
        if(itemDatabase == null)
        {
            itemDatabase = GetComponent<ItemDatabase>();
        }

        Instance = this;

        savePath =
            Application.persistentDataPath +
            "/save.json";

        
    }

    private void Start()
    {
        if (LoadGame)
        {
            Load();

            LoadGame = false;

            CanAutoSave = true;
        }
    }

    public void AutoSave()
    {

        if (!CanAutoSave)
        {
            return;
        }

        Debug.LogWarning(
        "AUTOSAVE CALLED",
        this
        );


        Save();

        Debug.Log("Autosaved");
    }

    public void Save()
    {


        SaveData data =
            new SaveData();

        Transform player =
            GameObject
            .FindGameObjectWithTag("Player")
            .transform;

        Debug.Log("Saving Position: " + player.position);


        data.playerX =
            player.position.x;

        data.playerY =
            player.position.y;

        data.playerZ =
            player.position.z;

        foreach (ItemData item in inventory.items)
        {
            data.inventoryItems.Add(
                item.itemID
            );
        }

        foreach (EquipmentSlot slot in equipmentManager.GetSlots())
        {
            ItemData item =
                slot.GetEquippedItem();

            if (item == null)
            {
                data.equippedItems.Add("");
            }
            else
            {
                data.equippedItems.Add(
                    item.itemID
                );
            }
        }

        string json =
            JsonUtility.ToJson(
                data,
                true
            );

        File.WriteAllText(
            savePath,
            json
        );

        Debug.Log("Game Saved");
        Debug.Log(json);
    }

    public void Load()
    {
        if (!File.Exists(savePath))
            return;

        CanAutoSave = false;

        string json =
            File.ReadAllText(savePath);

        SaveData data =
            JsonUtility.FromJson<SaveData>(
                json
            );

        // Load Position

        Transform player =
            GameObject
            .FindGameObjectWithTag("Player")
            .transform;

        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        player.position =
            new Vector3(
                data.playerX,
                data.playerY,
                data.playerZ
            );

        if (cc != null)
            cc.enabled = true;

        // Clear Inventory

        inventory.items.Clear();

        // Load Inventory

        foreach (
            string itemID
            in data.inventoryItems)
        {
            ItemData item =
                itemDatabase.GetItem(
                    itemID
                );

            if (item != null)
            {
                inventory.items.Add(
                    item
                );
            }
        }

        // Clear Equipment

        EquipmentSlot[] slots =
            equipmentManager.GetSlots();

        foreach (
            EquipmentSlot slot
            in slots)
        {
            slot.Unequip();
        }

        // Load Equipment

        for (
            int i = 0;
            i < data.equippedItems.Count &&
            i < slots.Length;
            i++)
        {
            string itemID =
                data.equippedItems[i];

            if (string.IsNullOrEmpty(itemID))
                continue;

            ItemData item =
                itemDatabase.GetItem(
                    itemID
                );

            if (item != null)
            {
                slots[i].Equip(item);
            }
        }

        inventoryUI.Refresh();

        Debug.Log(
            "Game Loaded"
        );

        CanAutoSave = true;
    }

    public static bool HasSaveFile()
    {
        string path =
            Application.persistentDataPath +
            "/save.json";

        return File.Exists(path);
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);

            Debug.Log("Save Deleted");
        }
    }
}