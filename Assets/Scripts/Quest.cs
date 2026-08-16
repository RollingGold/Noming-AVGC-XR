using System.Collections.Generic;

public class Quest
{
    public QuestData Data { get; }

    public List<QuestObjective> Objectives { get; }

    public bool IsCompleted { get; private set; }


    public Quest(QuestData data)
    {
        Data = data;

        Objectives = new List<QuestObjective>();

        foreach (QuestObjectiveData objectiveData in data.objectives)
        {
            Objectives.Add(
                new QuestObjective(objectiveData));
        }
    }

    public void CheckCompletion()
    {
        foreach (QuestObjective objective in Objectives)
        {
            if (!objective.IsCompleted)
                return;
        }

        IsCompleted = true;
    }
}