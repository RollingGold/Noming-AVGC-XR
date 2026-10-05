using System.Collections;
using UnityEngine;

public class EnableOnQuestComplete : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestData questData;

    [Header("Object")]
    [SerializeField] private MenuManager menuManager;

    [Header("Delay")]
    [SerializeField] private float delay = 3f;

    [SerializeField]
    private bool useUnscaledTime = true;

    private bool checking;
    private bool activated;


    private void Start()
    {
        menuManager.HideEnd();
    }


    private void Update()
    {
        if (activated)
            return;

        if (checking)
            return;

        if (QuestManager.Instance == null)
            return;

        if (questData == null)
            return;


        if (QuestManager.Instance.IsCompleted(questData))
        {
            StartCoroutine(
                EnableAfterDelay()
            );
        }
    }


    private IEnumerator EnableAfterDelay()
    {
        checking = true;


        if (useUnscaledTime)
        {
            float timer = 0f;

            while (timer < delay)
            {
                timer += Time.unscaledDeltaTime;

                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(delay);
        }


        menuManager.ShowEnd();


        activated = true;
    }
}