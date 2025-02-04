using UnityEngine;

public class TestTroop : BaseTroop
{
    protected override void Attack()
    {
        if (currentEnemy == null) return;

        isFigtning = true;

        if (currentEnemy.GetComponent<BaseEnemy>().currentTarget == null) currentEnemy.GetComponent<BaseEnemy>().RequestTarget(gameObject);

        currentEnemy.GetComponent<BaseEnemy>().TakeDamage(troopData.stats.damage, DamageTypes.PHYSICAL);
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
        if (currentEnemy == null) isFigtning = false;


        if (!ignoreEnemies)
        {
            if (currentEnemy == null || currentEnemy.GetComponent<BaseEnemy>().currentTarget != gameObject) FindNewEnemy();

            if (currentEnemy != null)
            {
                if (canAttack && Vector2.Distance(transform.position, currentEnemy.transform.position) <= troopData.stats.attackRange) Attack();
            }
            else
            {
                targetLocation = homeBase.GetComponent<Barracks>().RequestTroopRandezvousPoint(id);
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, targetLocation) < 0.1f)
            {
                ignoreEnemies = false;
                targetLocation = homeBase.GetComponent<Barracks>().RequestTroopRandezvousPoint(id);
            }
        }
    }
}
