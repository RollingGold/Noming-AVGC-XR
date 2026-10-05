using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    [Header("Scene Loader")]
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


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (sceneLoader == null)
        {
            sceneLoader =
                GetComponent<SceneLoader>();
        }

        if (inventory == null)
        {
            inventory =
                GetComponent<Inventory>();
        }

        if (inventoryUI == null)
        {
            inventoryUI =
                GetComponent<InventoryUI>();
        }

        if (equipmentManager == null)
        {
            equipmentManager =
                GetComponent<EquipmentManager>();
        }

        if (itemDatabase == null)
        {
            itemDatabase =
                GetComponent<ItemDatabase>();
        }

        savePath =
            Path.Combine(
                Application.persistentDataPath,
                "save.json");


        Debug.Log(
            "Save Path: " +
            savePath);
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (LoadGame)
        {
            Load();

            LoadGame = false;

            CanAutoSave = true;
        }
    }


    // =========================================================
    // AUTO SAVE
    // =========================================================

    public void AutoSave()
    {
        if (!CanAutoSave)
        {
            Debug.Log(
                "AutoSave blocked.");

            return;
        }

        Debug.Log(
            "AUTOSAVE CALLED",
            this);

        Save();

        Debug.Log(
            "Autosaved");
    }


    // =========================================================
    // SAVE
    // =========================================================

    public void Save()
    {
        if (inventory == null)
        {
            Debug.LogError(
                "Save failed: Inventory is missing.",
                this);

            return;
        }

        if (itemDatabase == null)
        {
            Debug.LogError(
                "Save failed: ItemDatabase is missing.",
                this);

            return;
        }

        SaveData data =
            new SaveData();


        // =====================================================
        // INVENTORY
        // =====================================================

        data.inventoryItems.Clear();

        Debug.Log(
            "Saving inventory. Item count: " +
            inventory.items.Count);


        foreach (ItemData item in inventory.items)
        {
            if (item == null)
            {
                Debug.LogWarning(
                    "Inventory contains a NULL item.",
                    this);

                continue;
            }


            if (string.IsNullOrEmpty(item.itemID))
            {
                Debug.LogError(
                    "Item has an empty Item ID: " +
                    item.name,
                    item);

                continue;
            }


            Debug.Log(
                "Saving Item: " +
                item.name +
                " | ID: " +
                item.itemID);


            data.inventoryItems.Add(
                item.itemID);
        }


        // =====================================================
        // EQUIPMENT
        // =====================================================

        if (equipmentManager != null)
        {
            EquipmentSlot[] slots =
                equipmentManager.GetSlots();


            foreach (EquipmentSlot slot in slots)
            {
                if (slot == null)
                {
                    data.equippedItems.Add("");
                    continue;
                }


                ItemData item =
                    slot.GetEquippedItem();


                if (item == null)
                {
                    data.equippedItems.Add("");
                }
                else
                {
                    data.equippedItems.Add(
                        item.itemID);
                }
            }
        }


        // =====================================================
        // CONVERT TO JSON
        // =====================================================

        string json =
            JsonUtility.ToJson(
                data,
                true);


        // =====================================================
        // WRITE FILE
        // =====================================================

        try
        {
            File.WriteAllText(
                savePath,
                json);
        }
        catch (System.Exception exception)
        {
            Debug.LogError(
                "Failed to save game:\n" +
                exception);

            return;
        }


        Debug.Log(
            "Game Saved");

        Debug.Log(
            json);
    }


    // =========================================================
    // LOAD
    // =========================================================

    public void Load()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log(
                "No save file found.");

            return;
        }


        if (inventory == null)
        {
            Debug.LogError(
                "Load failed: Inventory is missing.",
                this);

            return;
        }


        if (itemDatabase == null)
        {
            Debug.LogError(
                "Load failed: ItemDatabase is missing.",
                this);

            return;
        }


        CanAutoSave = false;


        string json;

        try
        {
            json =
                File.ReadAllText(
                    savePath);
        }
        catch (System.Exception exception)
        {
            Debug.LogError(
                "Failed to read save file:\n" +
                exception);

            CanAutoSave = true;

            return;
        }


        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError(
                "Save file is empty.");

            CanAutoSave = true;

            return;
        }


        SaveData data;

        try
        {
            data =
                JsonUtility.FromJson<SaveData>(
                    json);
        }
        catch (System.Exception exception)
        {
            Debug.LogError(
                "Failed to read save data:\n" +
                exception);

            CanAutoSave = true;

            return;
        }


        if (data == null)
        {
            Debug.LogError(
                "SaveData is NULL.");

            CanAutoSave = true;

            return;
        }


        // =====================================================
        // INVENTORY
        // =====================================================

        inventory.items.Clear();


        if (data.inventoryItems != null)
        {
            Debug.Log(
                "Loading inventory. Saved item count: " +
                data.inventoryItems.Count);


            foreach (
                string itemID
                in data.inventoryItems)
            {
                if (string.IsNullOrEmpty(itemID))
                {
                    Debug.LogWarning(
                        "Found empty Item ID in save.");

                    continue;
                }


                Debug.Log(
                    "Trying to load Item ID: " +
                    itemID);


                ItemData item =
                    itemDatabase.GetItem(
                        itemID);


                if (item != null)
                {
                    inventory.items.Add(
                        item);


                    Debug.Log(
                        "Loaded Item: " +
                        item.name +
                        " | ID: " +
                        item.itemID);
                }
                else
                {
                    Debug.LogError(
                        "ItemDatabase could not find Item ID: " +
                        itemID);
                }
            }
        }


        // =====================================================
        // EQUIPMENT
        // =====================================================

        if (equipmentManager != null)
        {
            EquipmentSlot[] slots =
                equipmentManager.GetSlots();


            // Clear equipment

            foreach (
                EquipmentSlot slot
                in slots)
            {
                if (slot != null)
                {
                    slot.Unequip();
                }
            }


            // Load equipment

            if (data.equippedItems != null)
            {
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
                            itemID);


                    if (item != null)
                    {
                        slots[i].Equip(
                            item);


                        Debug.Log(
                            "Loaded equipped item: " +
                            item.name);
                    }
                    else
                    {
                        Debug.LogError(
                            "Could not find equipped Item ID: " +
                            itemID);
                    }
                }
            }
        }

        Debug.Log(
    "Items after loading: " +
    inventory.items.Count);

        // =====================================================
        // REFRESH UI
        // =====================================================

        if (inventoryUI != null)
        {
            inventoryUI.Refresh();
        }


        Debug.Log(
            "Game Loaded");


        Debug.Log(
            "Final inventory count: " +
            inventory.items.Count);


        CanAutoSave = true;
    }


    // =========================================================
    // CHECK SAVE
    // =========================================================

    public static bool HasSaveFile()
    {
        string path =
            Path.Combine(
                Application.persistentDataPath,
                "save.json");


        return File.Exists(path);
    }


    // =========================================================
    // DELETE SAVE
    // =========================================================

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);

            Debug.Log(
                "Save Deleted");
        }
        else
        {
            Debug.Log(
                "No save file to delete.");
        }
    }
}