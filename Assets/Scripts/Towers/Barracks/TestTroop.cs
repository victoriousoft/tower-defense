using UnityEngine;

public class TestTroop : BaseTroop
{
	protected override void Attack()
	{
		if (currentEnemy == null)
			return;

		isFighting = true;

		currentEnemy.GetComponent<BaseEnemy>().TakeDamage(troopData.stats.damage, DamageTypes.PHYSICAL);
		canAttack = false;
		StartCoroutine(ResetAttackCooldown());
	}
}
