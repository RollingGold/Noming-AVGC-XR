using UnityEngine;

public class InteractionUIManager : MonoBehaviour
{
    public static InteractionUIManager Instance;

    [SerializeField] private Transform promptContainer;
    [SerializeField] private GameObject promptPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject CreatePrompt(string interactionText)
    {
        GameObject prompt =
            Instantiate(
                promptPrefab,
                promptContainer);

        prompt
            .GetComponent<InteractionPromptUI>()
            .Setup(interactionText);

        return prompt;
    }

    public void RemovePrompt(GameObject prompt)
    {
        if (prompt != null)
        {
            Destroy(prompt);
        }
    }
}