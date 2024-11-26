using UnityEngine;

public class TestTroop : BaseTroop
{
    protected override void Attack()
    {
        if (currentEnemy == null) return;
        if (currentEnemy.GetComponent<BaseEnemy>().currentTarget == null) currentEnemy.GetComponent<BaseEnemy>().RequestTarget(gameObject);

        currentEnemy.GetComponent<BaseEnemy>().TakeDamage(damage, DamageTypes.PHYSICAL);
        canAttack = false;
        StartCoroutine(ResetAttackCooldown());
    }

    protected override void FixedUpdate()
    {
        if (canAttack && currentEnemy == null)
        {
            Heal(10);
            canAttack = false;
            StartCoroutine(ResetAttackCooldown());
        }
        if (targetLocation != null) WalkTo(targetLocation);


        if (currentEnemy == null || currentEnemy.GetComponent<BaseEnemy>().currentTarget != gameObject) currentEnemy = FindNewEnemy();

        if (currentEnemy != null)
        {
            targetLocation = currentEnemy.transform.position;
            if (canAttack) Attack();
        }
        else
        {
            targetLocation = homeBase.GetComponent<Barracks>().RequestTroopRandezvousPoint(id);
        }
    }

}
