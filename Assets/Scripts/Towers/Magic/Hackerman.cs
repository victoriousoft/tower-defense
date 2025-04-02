using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hackerman : BaseEvolutionTower
{
	EnemyTypes[] targetEnemyTypes = new EnemyTypes[] { EnemyTypes.GROUND, EnemyTypes.FLYING };
	public Transform[] projectileOrigins;
	private float damageDealt = 0;
	private bool skillInUse = false;
	public GameObject debuffIcon;

	protected override void Start()
	{
		base.Start();
	}

	protected override IEnumerator Shoot(GameObject enemy)
	{
		if (skillInUse)
			yield break;
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
				0.35f,
				KillProjectile
			);

			yield return new WaitForSeconds(0.125f);
		}
	}

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	protected override IEnumerator Skill(GameObject enemy)
	{
		skillInUse = true;
		int head = 0;
		foreach (
			GameObject targetEnemy in TowerHelpers.GetEnemiesInRange(
				transform.position,
				towerData.evolutions[evolutionIndex].range,
				targetEnemyTypes
			)
		)
		{
			if (targetEnemy != null)
			{
				BaseEnemy enemyScript = targetEnemy.GetComponent<BaseEnemy>();
				enemyScript.TakeDamage(towerData.evolutions[evolutionIndex].damage / 3, DamageTypes.MAGIC);
				enemyScript.NerfResistance(0, 2, 20);
				enemyScript.NerfResistance(1, 2, 20);
				enemyScript.Slowdown(0.85f, towerData.evolutions[1].damage);
				GameObject icon = Instantiate(
					debuffIcon,
					targetEnemy.transform.position,
					Quaternion.identity,
					targetEnemy.transform
				);
				icon.GetComponent<SelfDestruct>().DestroySelf(20);

				yield return TowerHelpers.AnimateLaser(
					GetComponent<LineRenderer>(),
					projectileOrigins[head],
					targetEnemy,
					0.05f,
					KillProjectile
				);
				if (head < 2)
					head++;
				else
					head = 0;
				yield return new WaitForSeconds(0.1f);
			}
		}
		skillInUse = false;
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
