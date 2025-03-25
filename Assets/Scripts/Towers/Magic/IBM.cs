using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IBM : BaseEvolutionTower
{
	EnemyTypes[] targetEnemyTypes = new EnemyTypes[] { EnemyTypes.GROUND };

	protected override void Start()
	{
		base.Start();
	}

	protected override IEnumerator Shoot(GameObject enemy)
	{
		yield return new WaitForSeconds(0.2f);
		foreach (
			GameObject targetEnemy in TowerHelpers.GetEnemiesInRange(
				transform.position,
				towerData.levels[level].range,
				targetEnemyTypes
			)
		)
		{
			yield return new WaitForSeconds(0.1f);
			BaseEnemy enemyScript = targetEnemy.GetComponent<BaseEnemy>();
			enemyScript.TakeDamage(towerData.evolutions[1].damage, DamageTypes.MAGIC);
			enemyScript.Slowdown(3, towerData.evolutions[1].cooldown - 0.5f);
		}
		yield return null;
	}

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }
}
