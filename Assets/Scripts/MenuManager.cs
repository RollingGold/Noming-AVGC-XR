using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject pauseMenuUI;

    [Header("Extra UI")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject characterLights;

    private InputSystem_Actions inputActions;

    private bool inventoryPressed;
    private bool escapePressed;

    public bool IsInventoryOpen =>
        inventoryUI.activeSelf;

    public bool IsPaused =>
        pauseMenuUI.activeSelf;

    private void Awake()
    {
        inputActions =
            new InputSystem_Actions();
    }

    private void Start()
    {
        inventoryUI.SetActive(false);
        pauseMenuUI.SetActive(false);

        UpdateVisuals();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Inventory
            .performed +=
            ctx => inventoryPressed = true;

        inputActions.Player.Escape
            .performed +=
            ctx => escapePressed = true;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        if (inventoryPressed)
        {
            ToggleInventory();
            inventoryPressed = false;
        }

        if (escapePressed)
        {
            TogglePause();
            escapePressed = false;
        }
    }

    public void ToggleInventory()
    {
        if (IsPaused)
            return;

        inventoryUI.SetActive(
            !inventoryUI.activeSelf
        );

        UpdateVisuals();
        UpdateTimeScale();
    }

    public void TogglePause()
    {
        if (IsInventoryOpen)
        {
            inventoryUI.SetActive(false);

            UpdateVisuals();
            UpdateTimeScale();
            return;
        }

        pauseMenuUI.SetActive(
            !pauseMenuUI.activeSelf
        );

        UpdateVisuals();
        UpdateTimeScale();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);

        UpdateVisuals();
        UpdateTimeScale();
    }

    private void UpdateVisuals()
    {
        bool inventoryOpen =
            inventoryUI.activeSelf;

        characterLights.SetActive(
            inventoryOpen
        );

        mainUI.SetActive(
            !inventoryOpen
        );
    }

    private void UpdateTimeScale()
    {
        Time.timeScale =
            (inventoryUI.activeSelf ||
             pauseMenuUI.activeSelf)
            ? 0f
            : 1f;
    }
}