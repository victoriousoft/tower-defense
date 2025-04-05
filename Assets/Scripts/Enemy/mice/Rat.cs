using UnityEngine;

public class Rat : BaseEnemy
{
	protected override void Attack() { }

	protected override void UseAbility() { }

	void FixedUpdate()
	{
		base.FixedUpdate();

		if (health < enemyData.stats.maxHealth / 2 && attacksTroops)
		{
			RageForExit();
		}
	}
}
