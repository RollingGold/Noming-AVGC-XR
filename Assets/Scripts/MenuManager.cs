using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject questmenuUI;
    [SerializeField] private GameObject endUI;

    [Header("Extra UI")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject characterLights;

    private InputSystem_Actions inputActions;

    private bool inventoryPressed;
    private bool escapePressed;
    private bool questPressed;


    // =========================================================
    // PROPERTIES
    // =========================================================

    public bool IsInventoryOpen =>
        inventoryUI.activeSelf;

    public bool IsPaused =>
        pauseMenuUI.activeSelf;

    public bool IsQuestMenuOpen =>
        questmenuUI.activeSelf;

    public bool IsEndOpen =>
        endUI != null &&
        endUI.activeSelf;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        inputActions =
            new InputSystem_Actions();
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        inventoryUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        questmenuUI.SetActive(false);

        if (endUI != null)
        {
            endUI.SetActive(false);
        }

        UpdateVisuals();
        UpdateTimeScale();
        UpdateCursor();
    }


    // =========================================================
    // ENABLE
    // =========================================================

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Inventory.performed +=
            OnInventoryPressed;

        inputActions.Player.Escape.performed +=
            OnEscapePressed;

        inputActions.Player.Quest.performed +=
            OnQuestPressed;
    }


    // =========================================================
    // DISABLE
    // =========================================================

    private void OnDisable()
    {
        inputActions.Player.Inventory.performed -=
            OnInventoryPressed;

        inputActions.Player.Escape.performed -=
            OnEscapePressed;

        inputActions.Player.Quest.performed -=
            OnQuestPressed;

        inputActions.Disable();
    }


    // =========================================================
    // INPUT
    // =========================================================

    private void OnInventoryPressed(
        InputAction.CallbackContext context)
    {
        inventoryPressed = true;
    }


    private void OnEscapePressed(
        InputAction.CallbackContext context)
    {
        escapePressed = true;
    }


    private void OnQuestPressed(
        InputAction.CallbackContext context)
    {
        questPressed = true;
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (IsEndOpen)
            return;

        if (GameInputManager.InputLocked)
            return;


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


        if (questPressed)
        {
            ToggleQuestMenu();

            questPressed = false;
        }
    }


    // =========================================================
    // INVENTORY
    // =========================================================

    public void ToggleInventory()
    {
        if (IsPaused ||
            IsEndOpen)
            return;


        if (IsQuestMenuOpen)
        {
            questmenuUI.SetActive(false);
        }


        inventoryUI.SetActive(
            !inventoryUI.activeSelf
        );


        UpdateAll();
    }


    // =========================================================
    // PAUSE
    // =========================================================

    public void TogglePause()
    {
        if (IsEndOpen)
            return;


        if (IsInventoryOpen)
        {
            inventoryUI.SetActive(false);

            UpdateAll();

            return;
        }


        if (IsQuestMenuOpen)
        {
            questmenuUI.SetActive(false);

            UpdateAll();

            return;
        }


        pauseMenuUI.SetActive(
            !pauseMenuUI.activeSelf
        );


        UpdateAll();
    }


    // =========================================================
    // QUEST
    // =========================================================

    public void ToggleQuestMenu()
    {
        if (IsPaused ||
            IsEndOpen)
            return;


        if (IsInventoryOpen)
        {
            inventoryUI.SetActive(false);
        }


        questmenuUI.SetActive(
            !questmenuUI.activeSelf
        );


        UpdateAll();
    }


    // =========================================================
    // END SCREEN
    // =========================================================

    public void ShowEnd()
    {
        if (endUI == null)
            return;

        // Close every other menu.
        inventoryUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        questmenuUI.SetActive(false);

        // Clear pending inputs.
        inventoryPressed = false;
        escapePressed = false;
        questPressed = false;

        // Show ONLY the end screen.
        endUI.SetActive(true);

        // Unlock cursor.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Stop gameplay.
        Time.timeScale = 0f;

        UpdateVisuals();
    }


    // =========================================================
    // HIDE END SCREEN
    // =========================================================

    public void HideEnd()
    {
        if (endUI == null)
            return;


        endUI.SetActive(false);

        UpdateAll();
    }


    // =========================================================
    // RESUME
    // =========================================================

    public void Resume()
    {
        pauseMenuUI.SetActive(false);

        UpdateAll();
    }


    // =========================================================
    // UPDATE EVERYTHING
    // =========================================================

    private void UpdateAll()
    {
        UpdateVisuals();
        UpdateTimeScale();
        UpdateCursor();
    }


    // =========================================================
    // VISUALS
    // =========================================================

    private void UpdateVisuals()
    {
        bool menuOpen =
            inventoryUI.activeSelf ||
            questmenuUI.activeSelf;


        characterLights.SetActive(
            menuOpen);


        mainUI.SetActive(
            !menuOpen);
    }


    // =========================================================
    // TIME SCALE
    // =========================================================

    private void UpdateTimeScale()
    {
        bool menuOpen =
            inventoryUI.activeSelf ||
            questmenuUI.activeSelf ||
            pauseMenuUI.activeSelf ||
            IsEndOpen;


        Time.timeScale =
            menuOpen
                ? 0f
                : 1f;
    }


    // =========================================================
    // CURSOR
    // =========================================================

    private void UpdateCursor()
    {
        bool menuOpen =
            inventoryUI.activeSelf ||
            questmenuUI.activeSelf ||
            pauseMenuUI.activeSelf ||
            IsEndOpen;


        if (menuOpen)
        {
            // MENU / END SCREEN
            Cursor.lockState =
                CursorLockMode.None;

            Cursor.visible = true;
        }
        else
        {
            // GAMEPLAY
            Cursor.lockState =
                CursorLockMode.Locked;

            Cursor.visible = false;
        }
    }


    // =========================================================
    // APPLICATION FOCUS
    // =========================================================

    private void OnApplicationFocus(
        bool hasFocus)
    {
        if (!hasFocus)
            return;


        UpdateCursor();
    }
}