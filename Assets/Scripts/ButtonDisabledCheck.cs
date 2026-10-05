using UnityEngine;
using UnityEngine.UI;

public class ButtonDisabledCheck : MonoBehaviour
{
    [Header("Disabled Image")]
    [SerializeField] private GameObject disabledImage;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    private void Update()
    {
        
        if(button.interactable == false)
        {
            disabledImage.SetActive(true);
        }
        else
        {
            disabledImage.SetActive(false);
        }

    }




}
