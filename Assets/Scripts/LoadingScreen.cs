using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class LoadingScreen : MonoBehaviour
{
    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("References")]

    [SerializeField]
    private AutoGenerateAndBake autoGenerator;

    [SerializeField]
    private GameObject loadingScreen;

    [SerializeField]
    private TMP_Text progressText;

    [SerializeField]
    private TMP_Text statusText;

    [Header("Player Input")]
    [SerializeField]
    private PlayerInput playerInput;


    // =========================================================
    // SETTINGS
    // =========================================================

    [Header("Settings")]

    [SerializeField]
    private bool loadOnStart = true;

    [SerializeField]
    [Min(0.1f)]
    private float minimumDisplayTime = 1f;


    // =========================================================
    // STATE
    // =========================================================

    private float displayedProgress;

    private float startTime;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (!loadOnStart)
            return;

        ShowLoadingScreen();

        DisablePlayerInput();

        startTime =
            Time.time;

        StartCoroutine(
            LoadingRoutine());
    }


    // =========================================================
    // LOADING
    // =========================================================

    private IEnumerator LoadingRoutine()
    {
        if (autoGenerator == null)
        {
            Debug.LogError(
                "LoadingScreen: " +
                "AutoGenerateAndBake is not assigned.");

            yield break;
        }


        SetStatus(
            "Generating level...");


        // -----------------------------------------------------
        // WAIT FOR GENERATION TO START
        // -----------------------------------------------------

        while (!autoGenerator.IsGenerating &&
               !autoGenerator.IsGenerationFinished)
        {
            UpdateProgress();

            yield return null;
        }


        // -----------------------------------------------------
        // GENERATING
        // -----------------------------------------------------

        SetStatus(
            "Building level...");


        while (!autoGenerator.IsGenerationFinished)
        {
            SetTargetProgress(
                autoGenerator.Progress);

            UpdateProgress();

            yield return null;
        }


        // -----------------------------------------------------
        // GENERATION FINISHED
        // -----------------------------------------------------

        SetTargetProgress(80f);

        SetStatus(
            "Level generated...");


        yield return null;


        // -----------------------------------------------------
        // WAIT FOR BAKE
        // -----------------------------------------------------

        SetStatus(
            "Baking navigation...");


        while (autoGenerator.IsBaking)
        {
            SetTargetProgress(
                autoGenerator.Progress);

            UpdateProgress();

            yield return null;
        }


        // -----------------------------------------------------
        // WAIT UNTIL GENERATOR FINISHES
        // -----------------------------------------------------

        while (!autoGenerator.IsFinished)
        {
            SetTargetProgress(
                autoGenerator.Progress);

            UpdateProgress();

            yield return null;
        }


        // -----------------------------------------------------
        // COMPLETE
        // -----------------------------------------------------

        SetTargetProgress(100f);

        SetStatus(
            "Complete!");


        // Make sure the player actually sees 100%.

        while (displayedProgress < 100f)
        {
            UpdateProgress();

            yield return null;
        }


        // Minimum loading time.

        float elapsed =
            Time.time -
            startTime;


        if (elapsed < minimumDisplayTime)
        {
            yield return new WaitForSeconds(
                minimumDisplayTime -
                elapsed);
        }


        HideLoadingScreen();
    }


    // =========================================================
    // PROGRESS
    // =========================================================

    private float targetProgress;


    private void SetTargetProgress(
        float value)
    {
        targetProgress =
            Mathf.Clamp(
                value,
                0f,
                100f);
    }


    private void UpdateProgress()
    {
        displayedProgress =
            Mathf.MoveTowards(
                displayedProgress,
                targetProgress,
                100f *
                Time.deltaTime);


        if (progressText != null)
        {
            progressText.text =
                Mathf.RoundToInt(
                    displayedProgress) +
                "%";
        }
    }


    // =========================================================
    // STATUS
    // =========================================================

    private void SetStatus(
        string text)
    {
        if (statusText != null)
        {
            statusText.text =
                text;
        }
    }


    // =========================================================
    // SHOW
    // =========================================================

    public void ShowLoadingScreen()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }


        displayedProgress = 0f;
        targetProgress = 0f;


        if (progressText != null)
        {
            progressText.text =
                "0%";
        }


        if (statusText != null)
        {
            statusText.text =
                "Preparing...";
        }
    }


    // =========================================================
    // HIDE
    // =========================================================

    public void HideLoadingScreen()
    {
        EnablePlayerInput();

        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }


    private void DisablePlayerInput()
    {
        GameInputManager.LockInput();
    }


    private void EnablePlayerInput()
    {
        GameInputManager.UnlockInput();
    }
}