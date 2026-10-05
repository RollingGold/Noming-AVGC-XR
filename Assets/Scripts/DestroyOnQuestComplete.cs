using UnityEngine;

public class DestroyOnQuestComplete : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestData questData;
    [SerializeField] private QuestData lastQuestData;

    [Header("Object")]
    [SerializeField] private GameObject objectToDestroy;

    private bool checkedCompletion;


    private void Update()
    {
        if (checkedCompletion)
            return;

        if (QuestManager.Instance == null)
            return;

        if (questData == null)
            return;


        if (QuestManager.Instance.IsCompleted(questData))
        {
            checkedCompletion = true;

            QuestManager.Instance.AcceptQuest(lastQuestData);

            GameObject target =
                objectToDestroy != null
                    ? objectToDestroy
                    : gameObject;

            Destroy(target);
        }
    }
}