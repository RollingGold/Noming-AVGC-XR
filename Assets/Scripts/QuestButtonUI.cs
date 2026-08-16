using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text questName;
    [SerializeField] private Button button;

    private Quest quest;
    private QuestMenuUI menu;

    public void Setup(Quest quest, QuestMenuUI menu)
    {
        this.quest = quest;
        this.menu = menu;

        questName.text = quest.Data.questName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        menu.ShowQuest(quest);
    }
}