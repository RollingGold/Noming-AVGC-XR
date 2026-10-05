using System.Collections;
using UnityEngine;

public class Skill3SphereSpawner : MonoBehaviour
{
    [Header("Sphere")]
    [SerializeField] private GameObject spherePrefab;

    [Header("Scale")]
    [SerializeField] private Vector3 startScale = Vector3.one;
    [SerializeField] private Vector3 endScale = new Vector3(5f, 5f, 5f);

    [Header("Growth")]
    [SerializeField] private float scaleDuration = 2f;

    [Header("After Growth")]
    [SerializeField] private bool destroyAfterGrowth = true;
    [SerializeField] private float destroyDelay = 0f;


    // =========================================================
    // ACTIVATE SKILL
    // =========================================================

    public void ActivateSkill()
    {
        if (spherePrefab == null)
        {
            Debug.LogError(
                "Skill3SphereSpawner: " +
                "Sphere Prefab is not assigned.",
                this);

            return;
        }

        StartCoroutine(
            SpawnAndScaleRoutine());
    }


    // =========================================================
    // SPAWN + SCALE
    // =========================================================

    private IEnumerator SpawnAndScaleRoutine()
    {
        // -----------------------------------------------------
        // SPAWN
        // -----------------------------------------------------

        GameObject sphere =
            Instantiate(
                spherePrefab,
                transform.position,
                Quaternion.identity);


        // -----------------------------------------------------
        // START SCALE
        // -----------------------------------------------------

        sphere.transform.localScale =
            startScale;


        float timer = 0f;


        // -----------------------------------------------------
        // GROW
        // -----------------------------------------------------

        while (
            sphere != null &&
            timer < scaleDuration)
        {
            timer +=
                Time.deltaTime;


            float progress =
                timer /
                scaleDuration;


            progress =
                Mathf.Clamp01(
                    progress);


            sphere.transform.localScale =
                Vector3.Lerp(
                    startScale,
                    endScale,
                    progress);


            yield return null;
        }


        // Make sure it reaches exact final scale.
        if (sphere != null)
        {
            sphere.transform.localScale =
                endScale;
        }


        // -----------------------------------------------------
        // DESTROY
        // -----------------------------------------------------

        if (sphere != null &&
            destroyAfterGrowth)
        {
            Destroy(
                sphere,
                destroyDelay);
        }
    }
}