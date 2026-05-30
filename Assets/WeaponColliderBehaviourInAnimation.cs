using UnityEngine;

public class WeaponColliderBehaviourInAnimation
    : StateMachineBehaviour
{
    private Weapon weapon;

    private Collider weaponCollider;

    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        if (weapon == null)
        {
            weapon =
                animator.GetComponentInChildren<Weapon>();

            weaponCollider =
                weapon.GetComponent<Collider>();
        }

        weapon.ResetHitEnemies();

        weaponCollider.enabled = true;
    }

    public override void OnStateExit(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex)
    {
        weaponCollider.enabled = false;
    }
}