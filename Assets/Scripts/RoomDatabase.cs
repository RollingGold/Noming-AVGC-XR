using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Room Database",
    menuName = "Level Editor/Room Database")]
public class RoomDatabase : ScriptableObject
{
    public List<Room> rooms = new();
}
