using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject questmenuUI;

    [Header("Extra UI")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject characterLights;



    private InputSystem_Actions inputActions;

    private bool inventoryPressed;
    private bool escapePressed;
    private bool questPressed;

    public bool IsInventoryOpen =>
        inventoryUI.activeSelf;

    public bool IsPaused =>
        pauseMenuUI.activeSelf;

    public bool IsQuestMenuOpen =>
    questmenuUI.activeSelf;

    private void Awake()
    {
        inputActions =
            new InputSystem_Actions();
    }

    private void Start()
    {
        inventoryUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        questmenuUI.SetActive(false);

        UpdateVisuals();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Inventory.performed += ctx =>
        {
            inventoryPressed = true;
        };
        inputActions.Player.Escape.performed += ctx =>
        {
            escapePressed = true;
        };
        inputActions.Player.Quest.performed += ctx =>
        {
            questPressed = true;
        };
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
        if(questPressed)
        {
            ToggleQuestMenu();
            questPressed = false;
        }
    }

    public void ToggleInventory()
    {
        if (IsPaused)
            return;

        if (IsQuestMenuOpen)
            questmenuUI.SetActive(false);

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

        if (IsQuestMenuOpen)
        {
            questmenuUI.SetActive(false);

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
    public void ToggleQuestMenu()
    {
        if (IsPaused)
            return;

        if (IsInventoryOpen)
            inventoryUI.SetActive(false);

        questmenuUI.SetActive(
            !questmenuUI.activeSelf
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
        bool menuOpen =
            inventoryUI.activeSelf ||
            questmenuUI.activeSelf;

        characterLights.SetActive(menuOpen);

        mainUI.SetActive(!menuOpen);
    }

    private void UpdateTimeScale()
    {
        Time.timeScale =
            (inventoryUI.activeSelf ||
             questmenuUI.activeSelf ||
             pauseMenuUI.activeSelf)
            ? 0f
            : 1f;
    }
}