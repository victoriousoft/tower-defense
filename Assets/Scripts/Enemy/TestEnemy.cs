using UnityEngine;

public class TestEnemy : BaseEnemy
{
    protected override void Attack()
    {
        if (currentTarget == null) return;
        if (Vector3.Distance(transform.position, currentTarget.transform.position) > enemyData.stats.attackRange) return;

        currentTarget.GetComponent<BaseTroop>().TakeDamage(enemyData.stats.damage);
        canAttack = false;
        StartCoroutine(ResetAttackCooldown());
    }
}
