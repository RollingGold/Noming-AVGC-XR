using UnityEngine;

public enum ObjectiveType
{
    Kill,
    Collect,
    Talk,
    ReachArea,
    Interact
}

[System.Serializable]
public class QuestObjectiveData
{
    public ObjectiveType objectiveType;

    public string targetID;

    public int requiredAmount = 1;

    [TextArea]
    public string description;
}