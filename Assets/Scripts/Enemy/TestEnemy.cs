using UnityEngine;

public class TestEnemy : BaseEnemy
{
	public Animator animator;

	protected override void Attack()
	{
		if (
			currentTarget == null
			|| Vector3.Distance(transform.position, currentTarget.transform.position) > enemyData.stats.attackRange
		)
			return;

		animator.SetBool("attacking", true);
		currentTarget.GetComponent<BaseTroop>().TakeDamage(enemyData.stats.damage);
		canAttack = false;
		StartCoroutine(ResetAttackCooldown());
	}
}
