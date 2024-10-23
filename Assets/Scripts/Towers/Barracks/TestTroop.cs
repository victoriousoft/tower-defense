public class TestTroop : BaseTroop
{
    protected override void Attack()
    {
        if (currentEnemy == null) return;
        if (currentEnemy.GetComponent<Health>().currentTarget == null) currentEnemy.GetComponent<Health>().currentTarget = gameObject;

        currentEnemy.GetComponent<Health>().TakeDamage(damage, DamageTypes.PHYSICAL);
        canAttack = false;
        StartCoroutine(ResetAttackCooldown());
    }

    protected override void FixedUpdate()
    {
        if (targetLocation != null) WalkTo(targetLocation);


        if (currentEnemy == null) currentEnemy = FindNewEnemy();
        if (currentEnemy != null)
        {
            currentEnemy.GetComponent<Movement>().isPaused = true;
            targetLocation = currentEnemy.transform.position;
            if (canAttack) Attack();
        }
        else
        {
            targetLocation = homeBase.GetComponent<Barracks>().RequestTroopRandezvousPoint(id);
        }
    }

}
