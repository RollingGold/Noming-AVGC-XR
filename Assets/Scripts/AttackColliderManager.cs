using System.Collections.Generic;
using UnityEngine;

public class AttackColliderManager : MonoBehaviour
{
    private Player player;
    private PlayerCombat playerCombat;

    private HashSet<Enemy> hitEnemies =
        new HashSet<Enemy>();


    private void Awake()
    {
        player =
            GetComponentInParent<Player>();

        playerCombat =
            GetComponentInParent<PlayerCombat>();

        ResetHitEnemies();
    }


    public void ResetHitEnemies()
    {
        hitEnemies.Clear();
    }


    private void OnTriggerEnter(Collider other)
    {
        TryHitEnemy(other);
    }


    private void OnTriggerStay(Collider other)
    {
        TryHitEnemy(other);
    }


    private void TryHitEnemy(Collider other)
    {
        Enemy enemy =
            other.GetComponentInParent<Enemy>();


        if (enemy == null)
            return;


        // Prevent multiple hits from the
        // same attack.
        if (hitEnemies.Contains(enemy))
            return;


        if (player == null)
        {
            Debug.LogError(
                "Weapon could not find Player.",
                this);

            return;
        }


        if (playerCombat == null)
        {
            Debug.LogError(
                "Weapon could not find PlayerCombat.",
                this);

            return;
        }


        hitEnemies.Add(enemy);


        // Calculate final damage.
        float damage =
            player.AttackDamage *
            (playerCombat.AttackPower / 100f);


        Debug.Log(
            "Weapon hit: " +
            enemy.name +
            " | Damage: " +
            damage);


        enemy.TakeDamage(damage);
    }
}