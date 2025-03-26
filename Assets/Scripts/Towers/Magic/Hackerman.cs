using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hackerman : BaseEvolutionTower
{
	public Transform[] projectileOrigins;
	private float damageDealt = 0;

	protected override void Start()
	{
		base.Start();
	}

	protected override IEnumerator Shoot(GameObject enemy)
	{
		for (int i = 0; i < projectileOrigins.Length; i++)
		{
			if (enemy == null)
			{
				if (
					TowerHelpers
						.GetEnemiesInRange(transform.position, towerData.levels[level].range, towerData.enemyTypes)
						.Length > 0
				)
				{
					enemy = TowerHelpers.SelectEnemyToAttack(
						TowerHelpers.GetEnemiesInRange(
							transform.position,
							towerData.levels[level].range,
							towerData.enemyTypes
						),
						targetType
					);
				}
				else
				{
					break;
				}
			}
			yield return TowerHelpers.AnimateLaser(
				GetComponent<LineRenderer>(),
				projectileOrigins[i],
				enemy,
				0.5f,
				KillProjectile
			);

			yield return new WaitForSeconds(0.5f);
		}
	}

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	protected override void KillProjectile(GameObject sphere, GameObject enemy, Vector3 enemyPosition)
	{
		if (sphere != null)
		{
			Destroy(sphere);
			GetComponent<Animator>().SetTrigger("idle");
		}
		else if (enemy != null)
			enemy.GetComponent<BaseEnemy>().TakeDamage(towerData.evolutions[0].damage / 50, DamageTypes.MAGIC);
		damageDealt += towerData.evolutions[0].damage / 50;
	}
}
