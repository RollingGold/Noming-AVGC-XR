using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private TMP_Text keybindText;
    [SerializeField] private TMP_Text interactionText;
    [SerializeField] private GameObject keybindRoot;

    [SerializeField]
    private InputActionReference interactAction;

    public void Setup(string text)
    {
        interactionText.text = text;

        keybindText.text =
            InputBindingUtility.GetPreferredBinding(
                interactAction.action);
    }

    public void SetSelected(bool selected)
    {
        keybindRoot.SetActive(selected);
    }
}