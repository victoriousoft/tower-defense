using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleEastBomber : BaseEvolutionTower
{
	private GameObject[] currentBombers = new GameObject[3];
	public GameObject[] bomberWaitPoints = new GameObject[3];
	public GameObject bomberPrefab;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(SpawnBomber());
	}

	void Update()
	{
		//check jestli neni v blizkosti holy order a pokud ano, tak friendly fire
	}

	protected override IEnumerator Shoot(GameObject enemy)
	{
		if (enemy.GetComponent<BaseEnemy>().enemyData.enemyType != EnemyTypes.GROUND)
			yield return null;

		for (int i = 0; i < currentBombers.Length; i++)
		{
			if (currentBombers[i] != null)
			{
				StartCoroutine(
					currentBombers[i]
						.GetComponent<HomingMissile>()
						.MoveToTarget(enemy, towerData.evolutions[0].damage, 5f)
				);
				currentBombers[i] = null;
			}
		}
		yield return null;
	}

	IEnumerator SpawnBomber()
	{
		for (int i = 0; i < currentBombers.Length; i++)
		{
			if (currentBombers[i] == null)
			{
				GameObject newBomber = Instantiate(bomberPrefab, transform.position, Quaternion.identity);
				currentBombers[i] = newBomber;
				StartCoroutine(newBomber.GetComponent<HomingMissile>().MoveToTarget(bomberWaitPoints[i], 0, 5f));
				break;
			}
		}
		yield return new WaitForSeconds(towerData.evolutions[0].cooldown);
		StartCoroutine(SpawnBomber());
	}

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	protected override IEnumerator Skill(GameObject enemy)
	{
		yield return null;
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }
}
