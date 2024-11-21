public class TestTroop : BaseTroop
{
    protected override void Attack()
    {
        if (currentEnemy == null) return;
        if (currentEnemy.GetComponent<BaseEnemy>().currentTarget == null) currentEnemy.GetComponent<BaseEnemy>().currentTarget = gameObject;

        currentEnemy.GetComponent<BaseEnemy>().TakeDamage(damage, DamageTypes.PHYSICAL);
        canAttack = false;
        StartCoroutine(ResetAttackCooldown());
    }

    protected override void FixedUpdate()
    {
        if (targetLocation != null) WalkTo(targetLocation);


        if (currentEnemy == null || currentEnemy.GetComponent<BaseEnemy>().currentTarget != gameObject) currentEnemy = FindNewEnemy();
        if (currentEnemy != null)
        {
            currentEnemy.GetComponent<BaseEnemy>().isPaused = true;
            targetLocation = currentEnemy.transform.position;
            if (canAttack) Attack();
        }
        else
        {
            targetLocation = homeBase.GetComponent<Barracks>().RequestTroopRandezvousPoint(id);
        }
    }

}
