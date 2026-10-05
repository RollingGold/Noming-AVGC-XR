using System.Collections;
using UnityEngine;
using Unity.AI.Navigation;

public class AutoGenerateAndBake : MonoBehaviour
{
    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("References")]

    [SerializeField]
    private LevelEditorSelection levelEditor;

    [SerializeField]
    private NavMeshSurface navMeshSurface;


    // =========================================================
    // SETTINGS
    // =========================================================

    [Header("Startup")]

    [SerializeField]
    private bool generateOnStart = true;

    [SerializeField]
    private bool bakeAfterGeneration = true;

    [SerializeField]
    [Min(0)]
    private int framesBeforeBake = 1;


    // =========================================================
    // STATE
    // =========================================================

    private bool started;
    private bool baking;
    private bool generationFinished;
    private bool generationStarted;

    private float progress;

    public bool IsGenerating =>
        generationStarted && !generationFinished;

    public bool IsGenerationFinished =>
        generationFinished;

    public bool IsBaking =>
        baking;

    public bool IsFinished =>
        generationFinished &&
        !baking;

    public float Progress =>
        progress;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (!generateOnStart)
        {
            generationFinished = true;
            progress = 100f;
            return;
        }

        StartCoroutine(
            GenerateAndBakeRoutine());
    }


    // =========================================================
    // GENERATE + BAKE
    // =========================================================

    private IEnumerator GenerateAndBakeRoutine()
    {
        if (started)
            yield break;

        started = true;

        progress = 0f;
        generationStarted = false;
        generationFinished = false;


        // -----------------------------------------------------
        // CHECK LEVEL EDITOR
        // -----------------------------------------------------

        if (levelEditor == null)
        {
            Debug.LogError(
                "AutoGenerateAndBake: " +
                "LevelEditorSelection is not assigned.");

            yield break;
        }


        // -----------------------------------------------------
        // CHECK NAVMESH
        // -----------------------------------------------------

        if (navMeshSurface == null)
        {
            Debug.LogError(
                "AutoGenerateAndBake: " +
                "NavMeshSurface is not assigned.");

            yield break;
        }


        // -----------------------------------------------------
        // START GENERATION
        // -----------------------------------------------------

        Debug.Log(
            "====================================");

        Debug.Log(
            "STARTING AUTOMATIC LEVEL GENERATION");

        Debug.Log(
            "====================================");


        generationStarted = true;

        progress = 5f;


        levelEditor.AutoBuild();


        // -----------------------------------------------------
        // WAIT ONE FRAME
        // -----------------------------------------------------

        yield return null;


        // -----------------------------------------------------
        // GENERATION
        // -----------------------------------------------------

        progress = 10f;


        while (levelEditor.IsAutoBuildRunning)
        {
            /*
             * We cannot know the exact AutoBuild percentage
             * unless AutoBuild exposes its own progress.
             *
             * Slowly move toward 80% while it is running.
             */

            progress =
                Mathf.MoveTowards(
                    progress,
                    80f,
                    Time.deltaTime * 5f);

            yield return null;
        }


        // -----------------------------------------------------
        // GENERATION COMPLETE
        // -----------------------------------------------------

        progress = 80f;

        generationFinished = true;


        Debug.Log(
            "====================================");

        Debug.Log(
            "LEVEL GENERATION COMPLETE");

        Debug.Log(
            "====================================");


        // -----------------------------------------------------
        // SETTLE
        // -----------------------------------------------------

        for (
            int i = 0;
            i < framesBeforeBake;
            i++)
        {
            yield return null;
        }


        Physics.SyncTransforms();


        // -----------------------------------------------------
        // BAKE
        // -----------------------------------------------------

        if (bakeAfterGeneration)
        {
            yield return StartCoroutine(
                BakeNavMeshRoutine());
        }
        else
        {
            progress = 100f;
        }
    }


    // =========================================================
    // BAKE ROUTINE
    // =========================================================

    private IEnumerator BakeNavMeshRoutine()
    {
        baking = true;

        progress = 85f;


        Debug.Log(
            "====================================");

        Debug.Log(
            "BAKING NAVMESH");

        Debug.Log(
            "====================================");


        Physics.SyncTransforms();


        // BuildNavMesh is synchronous.
        // So we display a progress stage before
        // and after it rather than pretending we know
        // its internal percentage.

        progress = 90f;

        yield return null;


        navMeshSurface.BuildNavMesh();


        progress = 100f;


        baking = false;


        Debug.Log(
            "====================================");

        Debug.Log(
            "NAVMESH BAKE COMPLETE");

        Debug.Log(
            "====================================");
    }


    // =========================================================
    // MANUAL BAKE
    // =========================================================

    public void BakeNavMesh()
    {
        if (baking)
        {
            Debug.LogWarning(
                "NavMesh is already being baked.");

            return;
        }

        if (navMeshSurface == null)
        {
            Debug.LogError(
                "AutoGenerateAndBake: " +
                "NavMeshSurface is not assigned.");

            return;
        }

        StartCoroutine(
            BakeNavMeshRoutine());
    }


    // =========================================================
    // MANUAL REBUILD
    // =========================================================

    public void RebuildNavMesh()
    {
        BakeNavMesh();
    }
}