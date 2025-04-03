using UnityEngine;

public class BlackDog : BaseEnemy
{
	protected override void Attack()
	{
		if (
			currentTarget == null
			|| Vector3.Distance(transform.position, currentTarget.transform.position) > enemyData.stats.attackRange
		)
		{
			if (!isIdle)
			{
				if (transform.position.x > currentTarget.transform.position.x)
					GetComponentInChildren<SpriteRenderer>().flipX = true;
				animator.SetBool("idle", true);
				animator.SetBool("stop", true);
				isIdle = true;
			}
			return;
		}
		else
			animator.SetBool("stop", false);

		animator.SetBool("idle", true);
		animator.SetTrigger("attack");
		if (transform.position.x > currentTarget.transform.position.x)
			GetComponentInChildren<SpriteRenderer>().flipX = true;
		currentTarget.GetComponent<BaseTroop>().TakeDamage(enemyData.stats.damage);
		canAttack = false;
		StartCoroutine(ResetAttackCooldown());
	}
}
