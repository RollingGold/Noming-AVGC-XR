using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Quest",
    menuName = "Quest/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Info")]
    public string questID;

    public string questName;

    [TextArea]
    public string description;

    [Header("Objectives")]
    public List<QuestObjectiveData> objectives =
        new();

    [Header("Rewards")]
    public int goldReward;

    public int xpReward;

    public List<ItemData> itemRewards =
        new();
}