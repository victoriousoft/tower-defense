using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IBM : BaseEvolutionTower
{
	EnemyTypes[] targetEnemyTypes = new EnemyTypes[] { EnemyTypes.GROUND };
	public Animator circleAnimator;
	public GameObject buffIcon;

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
			circleAnimator.SetTrigger("attack");
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
		circleAnimator.SetTrigger("ability");
		foreach (GameObject tower in GameObject.FindGameObjectsWithTag("Tower"))
		{
			Debug.Log(tower.name);
			if (
				Vector2.Distance(tower.transform.position, transform.position)
					< towerData.evolutions[evolutionIndex].range
				&& tower != this.gameObject
			)
			{
				tower
					.GetComponent<BaseTower>()
					.StartCoroutine(tower.GetComponent<BaseTower>().EnhanceTemporarily(1.5f, 17));

				GameObject icon = Instantiate(buffIcon, tower.transform.position, Quaternion.identity, tower.transform);
				icon.GetComponent<SelfDestruct>().DestroySelf(17);
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
