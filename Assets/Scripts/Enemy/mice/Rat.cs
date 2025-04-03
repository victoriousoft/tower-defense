using UnityEngine;

public class Rat : BaseEnemy
{
	protected override void Attack() { }

	protected override void UseAbility() { }

	void Update()
	{
		if (health < enemyData.stats.maxHealth / 3 && attacksTroops)
		{
			attacksTroops = false;
			currentSpeed *= 1.666f;
			animator.SetTrigger("rage");
		}
	}
}
