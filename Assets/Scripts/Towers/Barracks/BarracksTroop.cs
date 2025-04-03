using UnityEngine;

public class BarracksTroop : BaseTroop
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
