using UnityEngine;

public class QuestGiver : MonoBehaviour, IInteractable
{
    [Header("Quest")]
    [SerializeField]
    private QuestData quest;

    [Header("Interaction")]
    [SerializeField]
    private string interactionText = "Accept Quest";

    public string InteractionText => interactionText;

    public void Interact()
    {
        if (quest == null)
            return;

        if (QuestManager.Instance.HasQuest(quest))
        {
            Debug.Log("Quest already accepted.");
            return;
        }

        if (QuestManager.Instance.IsCompleted(quest))
        {
            Debug.Log("Quest already completed.");
            return;
        }

        QuestManager.Instance.AcceptQuest(quest);
    }
}
