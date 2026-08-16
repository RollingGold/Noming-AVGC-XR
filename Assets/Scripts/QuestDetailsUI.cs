using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestDetailsUI : MonoBehaviour
{
    public static QuestDetailsUI Instance;

    [Header("UI")]
    [SerializeField] private TMP_Text questName;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text objectives;
    [SerializeField] private TMP_Text rewards;

    [Header("Buttons")]
    [SerializeField] private Button trackButton;

    private Quest currentQuest;

    private void Awake()
    {
        Clear();

        trackButton.onClick.AddListener(TrackQuest);
    }

    public void Setup(Quest quest)
    {
        currentQuest = quest;

        questName.text = quest.Data.questName;

        description.text = quest.Data.description;

        objectives.text = "";

        foreach (QuestObjective objective in quest.Objectives)
        {
            objectives.text +=
                $"• {objective.Data.description} " +
                $"({objective.CurrentAmount}/{objective.Data.requiredAmount})\n";
        }

        rewards.text =
            $"XP: {quest.Data.xpReward}\n" +
            $"Gold: {quest.Data.goldReward}";
    }

    private void TrackQuest()
    {
        if (currentQuest == null)
            return;

        QuestManager.Instance.TrackQuest(currentQuest);

        QuestMenuUI.Instance.ScrollReset();
    }

  

    public void Clear()
    {
        currentQuest = null;

        questName.text = "";
        description.text = "";
        objectives.text = "";
        rewards.text = "";
    }
}