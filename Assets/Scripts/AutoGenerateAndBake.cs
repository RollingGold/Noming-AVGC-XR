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

    [Tooltip(
        "Automatically generate the level when the scene starts.")]
    [SerializeField]
    private bool generateOnStart = true;

    [Tooltip(
        "Bake the NavMesh after Auto Build finishes.")]
    [SerializeField]
    private bool bakeAfterGeneration = true;

    [Tooltip(
        "Wait this many frames after generation before baking.")]
    [Min(0)]
    [SerializeField]
    private int framesBeforeBake = 1;


    // =========================================================
    // STATE
    // =========================================================

    private bool started;

    private bool baking;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        if (!generateOnStart)
            return;

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
        // START AUTO BUILD
        // -----------------------------------------------------

        Debug.Log(
            "====================================");

        Debug.Log(
            "STARTING AUTOMATIC LEVEL GENERATION");

        Debug.Log(
            "====================================");


        levelEditor.AutoBuild();


        // -----------------------------------------------------
        // WAIT FOR AUTO BUILD TO ACTUALLY START
        // -----------------------------------------------------

        yield return null;


        // -----------------------------------------------------
        // WAIT UNTIL AUTO BUILD FINISHES
        // -----------------------------------------------------

        while (levelEditor.IsAutoBuildRunning)
        {
            yield return null;
        }


        // -----------------------------------------------------
        // LET DESTROYED / CREATED OBJECTS SETTLE
        // -----------------------------------------------------

        for (
            int i = 0;
            i < framesBeforeBake;
            i++)
        {
            yield return null;
        }


        // -----------------------------------------------------
        // PHYSICS / TRANSFORM UPDATE
        // -----------------------------------------------------

        Physics.SyncTransforms();


        // -----------------------------------------------------
        // BAKE NAVMESH
        // -----------------------------------------------------

        if (bakeAfterGeneration)
        {
            BakeNavMesh();
        }
    }


    // =========================================================
    // BAKE
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

        baking = true;

        Debug.Log(
            "====================================");

        Debug.Log(
            "BAKING NAVMESH");

        Debug.Log(
            "====================================");


        // -----------------------------------------------------
        // Make sure generated transforms are updated.
        // -----------------------------------------------------

        Physics.SyncTransforms();


        // -----------------------------------------------------
        // Build NavMesh from generated rooms.
        // -----------------------------------------------------

        navMeshSurface.BuildNavMesh();


        baking = false;


        Debug.Log(
            "====================================");

        Debug.Log(
            "NAVMESH BAKE COMPLETE");

        Debug.Log(
            "====================================");
    }


    // =========================================================
    // MANUAL REBUILD NAVMESH
    // =========================================================

    public void RebuildNavMesh()
    {
        BakeNavMesh();
    }


    // =========================================================
    // PUBLIC STATE
    // =========================================================

    public bool IsBaking =>
        baking;
}