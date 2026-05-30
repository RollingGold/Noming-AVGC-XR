using UnityEngine;

public class NormalAttackCollider : MonoBehaviour
{
    private Enemy enemy;
    private Player player;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();

        
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
           
            player =  other.GetComponent<Player>();

            player.TakeDamage((int)enemy.AttackDamage);

        }
    }
}
