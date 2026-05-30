using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Player player;

    private Collider weaponCollider;

    private HashSet<Enemy> hitEnemies =
        new HashSet<Enemy>();

    private void Awake()
    {
        player = GetComponentInParent<Player>();

        ResetHitEnemies();

    }

    public void ResetHitEnemies()
    {
        hitEnemies.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy =
            other.GetComponent<Enemy>();

        if (enemy == null)
            return;

        if (hitEnemies.Contains(enemy))
            return;

        hitEnemies.Add(enemy);

        enemy.TakeDamage(player.AttackDamage);
    }
}