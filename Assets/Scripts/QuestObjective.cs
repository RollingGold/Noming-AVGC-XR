public class QuestObjective
{
    public QuestObjectiveData Data { get; }

    public int CurrentAmount { get; private set; }

    public bool IsCompleted =>
        CurrentAmount >= Data.requiredAmount;

    public QuestObjective(QuestObjectiveData data)
    {
        Data = data;
    }

    public void AddProgress(int amount = 1)
    {
        CurrentAmount += amount;

        if (CurrentAmount > Data.requiredAmount)
        {
            CurrentAmount = Data.requiredAmount;
        }
    }
}
