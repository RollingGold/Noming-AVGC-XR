using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenuUI : MonoBehaviour
{
    public static QuestMenuUI Instance;

    [Header("Quest List")]
    [SerializeField] private ScrollRect scrollRectQuestList;
    [SerializeField] private Transform questListContent;
    [SerializeField] private GameObject questButtonPrefab;

    [Header("Quest Details")]
    [SerializeField] private QuestDetailsUI questDetails;
    [SerializeField] private ScrollRect scrollRectQuestDetail;
    [SerializeField] private RectTransform descriptionContent;
    [SerializeField] private float scrollSpeed = 8f;

    private Coroutine questListScrollRoutine;
    private Coroutine questDetailScrollRoutine;

    private readonly List<GameObject> spawnedButtons =
        new();

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        ClearButtons();

        List<Quest> quests =
            QuestManager.Instance.GetActiveQuests();

        foreach (Quest quest in quests)
        {
            GameObject button =
                Instantiate(
                    questButtonPrefab,
                    questListContent);

            button.GetComponent<QuestButtonUI>()
                .Setup(quest, this);

            spawnedButtons.Add(button);
        }

        if (quests.Count > 0)
        {
            ShowQuest(quests[0]);
        }
        else
        {
            questDetails.Clear();
        }

        ScrollReset();
    }

    public void ShowQuest(Quest quest)
    {
        questDetails.Setup(quest);

        ScrollReset();

        StopAllCoroutines();
        StartCoroutine(RefreshLayout());
    }

    private void ClearButtons()
    {
        foreach (GameObject button in spawnedButtons)
        {
            Destroy(button);
        }

        spawnedButtons.Clear();
    }

    public void ScrollReset()
    {
        if (questListScrollRoutine != null)
            StopCoroutine(questListScrollRoutine);

        if (questDetailScrollRoutine != null)
            StopCoroutine(questDetailScrollRoutine);

        questListScrollRoutine =
            StartCoroutine(SmoothScrollToTop(scrollRectQuestList));

        questDetailScrollRoutine =
            StartCoroutine(SmoothScrollToTop(scrollRectQuestDetail));
    }

    private IEnumerator SmoothScrollToTop(ScrollRect scrollRect)
    {
        while (scrollRect.verticalNormalizedPosition < 0.999f)
        {
            scrollRect.verticalNormalizedPosition =
                Mathf.Lerp(
                    scrollRect.verticalNormalizedPosition,
                    1f,
                    Time.unscaledDeltaTime * scrollSpeed);

            yield return null;
        }

        scrollRect.verticalNormalizedPosition = 1f;
    }

    private IEnumerator RefreshLayout()
    {
        yield return null;

        Canvas.ForceUpdateCanvases();

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            descriptionContent);

        scrollRectQuestDetail.verticalNormalizedPosition = 1f;
    }
}