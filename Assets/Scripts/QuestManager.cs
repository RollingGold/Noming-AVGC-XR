using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private readonly List<Quest> activeQuests =
        new();

    private readonly List<Quest> completedQuests =
        new();

    [SerializeField]
    private List<QuestData> testQuests = new();
    [ContextMenu("Fill Quest List")]
    private void FillQuestList()
    {
        activeQuests.Clear();

        foreach (QuestData questData in testQuests)
        {
            activeQuests.Add(new Quest(questData));
        }

        QuestMenuUI.Instance.Refresh();

        RefreshQuestUI();
    }

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #region Quest Management

    public void AcceptQuest(
        QuestData questData)
    {
        if (HasQuest(questData))
            return;

        Quest quest =
            new Quest(questData);

        activeQuests.Add(quest);

        RefreshQuestUI();
    }

    public void CompleteQuest(
        Quest quest)
    {
        if (!activeQuests.Contains(quest))
            return;

        activeQuests.Remove(quest);

        completedQuests.Add(quest);

        GiveRewards(quest);

        RefreshQuestUI();
    }

    #endregion

    #region Rewards

    private void GiveRewards(
        Quest quest)
    {
        Debug.Log(
            "Gold : " +
            quest.Data.goldReward);

        Debug.Log(
            "XP : " +
            quest.Data.xpReward);

        // Inventory rewards later
        // Experience later
    }

    #endregion

    #region Utility

    public bool HasQuest(
        QuestData questData)
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.Data == questData)
                return true;
        }

        return false;
    }

    public bool IsCompleted(
        QuestData questData)
    {
        foreach (Quest quest in completedQuests)
        {
            if (quest.Data == questData)
                return true;
        }

        return false;
    }

    public List<Quest> GetActiveQuests()
    {
        return activeQuests;
    }

    #endregion

    #region Progress

    public void EnemyKilled(string enemyID)
    {
        UpdateObjectives(
            ObjectiveType.Kill,
            enemyID);
    }

    public void ItemCollected(string itemID)
    {
        UpdateObjectives(
            ObjectiveType.Collect,
            itemID);
    }

    public void NPCTalked(string npcID)
    {
        UpdateObjectives(
            ObjectiveType.Talk,
            npcID);
    }

    public void AreaReached(string areaID)
    {
        UpdateObjectives(
            ObjectiveType.ReachArea,
            areaID);
    }

    public void ObjectInteracted(string objectID)
    {
        UpdateObjectives(
            ObjectiveType.Interact,
            objectID);
    }

    private void UpdateObjectives(
    ObjectiveType objectiveType,
    string targetID)
    {
        List<Quest> completedThisFrame =
            new();

        foreach (Quest quest in activeQuests)
        {
            foreach (QuestObjective objective in quest.Objectives)
            {
                if (objective.IsCompleted)
                    continue;

                if (objective.Data.objectiveType != objectiveType)
                    continue;

                if (objective.Data.targetID != targetID)
                    continue;

                objective.AddProgress();

                RefreshQuestUI();

                quest.CheckCompletion();

                if (quest.IsCompleted)
                {
                    completedThisFrame.Add(
                        quest);
                }
            }
        }

        foreach (Quest quest in completedThisFrame)
        {
            CompleteQuest(quest);
        }
    }

    #endregion



    public void RefreshQuestUI()
    {
        if (activeQuests.Count > 0)
        {
            QuestUI.Instance.Setup(activeQuests[0]);
        }
        else
        {
            QuestUI.Instance.NoQuestTextChanger();
        }    
    }

    public void TrackQuest(Quest quest)
    {
        if (!activeQuests.Contains(quest))
            return;

        activeQuests.Remove(quest);

        activeQuests.Insert(0, quest);

        QuestMenuUI.Instance.Refresh();

        RefreshQuestUI();
    }
}