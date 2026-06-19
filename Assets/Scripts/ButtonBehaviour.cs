using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ButtonButtonBehaviour : MonoBehaviour, 
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{


    [Header("Scale")]
    [SerializeField] private float hoverScale = 1.09f;
    [SerializeField] private float pressedScale = 0.93f;
    [SerializeField] private float speed = 9f;

    [Header("Imagae")]
    [SerializeField] private Sprite hoverImage;
    [SerializeField] private Image originalImage;

    private Sprite originalSprite;

    private Vector3 originalScale;

    private Vector3 targetScale;


    private Button button;

    private void Awake()
    {
        if (originalImage == null)
        {
            originalImage = GetComponent<Image>();
        }

        originalSprite = originalImage.sprite;

        originalScale = transform.localScale;

        targetScale = originalScale;

        button = GetComponent<Button>();
    }
   
    
    void Update()
    {

        enabled = button.interactable;

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * speed);
    }

    public void SizeReset()
    {
        transform.localScale = originalScale;
        targetScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;

        originalImage.sprite = hoverImage;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;

        originalImage.sprite = originalSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = originalScale * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;
    }

    private void OnDisable()
    {
        originalImage.sprite = originalSprite;

        SizeReset();
    }

}