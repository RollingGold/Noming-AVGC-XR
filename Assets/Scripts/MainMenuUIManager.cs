using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Continue")]
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject disabledOverlay;

  
    
    private void Update()
    {
        continueButton.interactable = SaveManager.HasSaveFile();

        if (continueButton.interactable)
        {
            disabledOverlay.SetActive(false);
        }
        else
        {
            disabledOverlay.SetActive(true);
        }
        
    }
}
