using UnityEngine;

public class TestEnemy : BaseEnemy
{
    protected override void Attack()
    {
        if (currentTarget == null) return;
        if (Vector3.Distance(transform.position, currentTarget.transform.position) > range) return;

        currentTarget.GetComponent<BaseTroop>().TakeDamage(damage);
        canAttack = false;
        StartCoroutine(ResetAttackCooldown());
    }
}
