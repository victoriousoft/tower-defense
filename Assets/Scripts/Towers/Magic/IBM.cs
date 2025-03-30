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
				towerData.evolutions[evolutionIndex].range,
				targetEnemyTypes
			)
		)
		{
			yield return new WaitForSeconds(0.05f);
			BaseEnemy enemyScript = targetEnemy.GetComponent<BaseEnemy>();
			enemyScript.TakeDamage(towerData.evolutions[1].damage, DamageTypes.MAGIC);
			enemyScript.Slowdown(3, towerData.evolutions[1].cooldown);
		}
		yield return null;
	}

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	protected override IEnumerator Skill(GameObject enemy)
	{
		foreach (GameObject tower in GameObject.FindGameObjectsWithTag("Tower"))
		{
			if (
				Vector2.Distance(tower.transform.position, transform.position)
				< towerData.evolutions[evolutionIndex].range
			)
			{
				tower.GetComponent<BaseTower>().EnhanceTemoprarily(2, 30);
			}
		}
		foreach (
			GameObject targetEnemy in TowerHelpers.GetEnemiesInRange(
				transform.position,
				towerData.evolutions[evolutionIndex].range,
				targetEnemyTypes
			)
		)
		{
			BaseEnemy enemyScript = targetEnemy.GetComponent<BaseEnemy>();
			enemyScript.Slowdown(1000, towerData.evolutions[1].cooldown * 2);
		}
		yield return null;
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }
}
