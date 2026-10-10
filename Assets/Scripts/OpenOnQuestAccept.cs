using UnityEngine;

public class OpenOnQuestAccept : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestData questData;
    [SerializeField] private QuestManager questManager;

    private DestroyOnInteract destroyOnInteract;

    private void Start()
    {
        destroyOnInteract = GetComponent<DestroyOnInteract>();
    }

    private void Update()
    {
        InteractOnQuestAccept();
    }



    private void InteractOnQuestAccept()
    {
        if (questManager.HasQuest(questData))
        {

            destroyOnInteract.Interact();
        }
    }


}
