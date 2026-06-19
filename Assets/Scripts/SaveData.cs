using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    // Position
    public float playerX;
    public float playerY;
    public float playerZ;

    // Inventory
    public List<string> inventoryItems =
        new List<string>();

    // Equipment
    public List<string> equippedItems =
        new List<string>();
}