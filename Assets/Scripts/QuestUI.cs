using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestUI : MonoBehaviour
{
    [Header("Quest text element")]
    [SerializeField] private TMP_Text questName;
    [SerializeField] private TMP_Text objectiveText;

    [Header("Control text")]
    [SerializeField] private TMP_Text questControlText;
    [SerializeField] private string questControlTextFirstHalf;
    [SerializeField] private string questControlTextSecondHalf;
    [SerializeField] private InputActionReference inputAction;

    public static QuestUI Instance;

    private void Awake()
    {
        Instance = this;

        questControlText.text = $"{questControlTextFirstHalf} {InputBindingUtility.GetPreferredBinding(inputAction.action)} {questControlTextSecondHalf}";
    }

    private void Start()
    {
        QuestManager.Instance.RefreshQuestUI();
    }

    public void Setup(Quest quest)
    {
        questName.text = quest.Data.questName;

        if (quest.Objectives.Count > 0)
        {
            QuestObjective objective =
                quest.Objectives[0];

            objectiveText.text =
                $"{objective.Data.description} " +
                $"({objective.CurrentAmount}/{objective.Data.requiredAmount})";
        }
    }
    public void Complete()
    {
        objectiveText.text =
            "<s>" +
            objectiveText.text +
            "</s>";
    }

    public void NoQuestTextChanger()
    { 
        questName.text = "No Active Quest";
        objectiveText.text = "Talk to an NPC";
    }

}