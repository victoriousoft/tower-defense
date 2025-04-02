using UnityEngine;

public class BlackDog : BaseEnemy
{
	public Animator animator;

	protected override void Attack()
	{
		if (
			currentTarget == null
			|| Vector3.Distance(transform.position, currentTarget.transform.position) > enemyData.stats.attackRange
		)
			return;

		animator.SetTrigger("attack");
		currentTarget.GetComponent<BaseTroop>().TakeDamage(enemyData.stats.damage);
		canAttack = false;
		StartCoroutine(ResetAttackCooldown());
	}
}
