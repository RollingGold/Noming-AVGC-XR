using System.Collections.Generic;
using UnityEngine;

public class Skill2SphereColliderManager : MonoBehaviour
{
    [Header("Fall")]
    [SerializeField] private float fallSpeed = 15f;
    [SerializeField] private float groundOffset = 2f;

    [Header("Destroy")]
    [SerializeField] private bool destroyOnGround = true;
    [SerializeField] private float destroyDelay = 0f;

    private Player player;
    private PlayerCombat playerCombat;

    private HashSet<Enemy> hitEnemies =
        new HashSet<Enemy>();

    private float targetY;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player =
                playerObject.GetComponentInParent<Player>();

            playerCombat =
                playerObject.GetComponentInParent<PlayerCombat>();
        }


        // Remember the ground position BEFORE
        // the sphere starts moving.
        targetY =
            transform.position.y -
            groundOffset;


        ResetHitEnemies();
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        Fall();
    }


    // =========================================================
    // FALL
    // =========================================================

    private void Fall()
    {
        Vector3 position =
            transform.position;


        position.y =
            Mathf.MoveTowards(
                position.y,
                targetY,
                fallSpeed *
                Time.deltaTime
            );


        transform.position =
            position;


        // Reached target height
        if (Mathf.Approximately(
            position.y,
            targetY))
        {
            OnGround();
        }
    }


    // =========================================================
    // ON GROUND
    // =========================================================

    private void OnGround()
    {
        if (!destroyOnGround)
            return;


        Destroy(
            gameObject,
            destroyDelay
        );
    }


    // =========================================================
    // RESET HIT ENEMIES
    // =========================================================

    public void ResetHitEnemies()
    {
        hitEnemies.Clear();
    }


    // =========================================================
    // COLLISION
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        TryHitEnemy(other);
    }


    private void OnTriggerStay(Collider other)
    {
        TryHitEnemy(other);
    }


    // =========================================================
    // DAMAGE
    // =========================================================

    private void TryHitEnemy(Collider other)
    {
        Enemy enemy =
            other.GetComponentInParent<Enemy>();


        if (enemy == null)
            return;

        // not working as intended
        //if (hitEnemies.Contains(enemy))
        //    return;


        if (player == null)
        {
            Debug.LogError(
                "Skill2SphereColliderManager: " +
                "Player could not be found.",
                this);

            return;
        }


        if (playerCombat == null)
        {
            Debug.LogError(
                "Skill2SphereColliderManager: " +
                "PlayerCombat could not be found.",
                this);

            return;
        }


        hitEnemies.Add(enemy);


        float damage =
            player.AttackDamage *
            (playerCombat.Skill2Power / 100f);


        Debug.Log(
            "Skill 2 hit: " +
            enemy.name +
            " | Damage: " +
            damage);


        enemy.TakeDamage(damage);
    }
}