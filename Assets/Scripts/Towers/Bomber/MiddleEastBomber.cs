using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleEastBomber : BaseEvolutionTower
{
	private GameObject[] currentBombers = new GameObject[3];
	public GameObject[] bomberWaitPoints = new GameObject[3];
	public Transform spawnPosition;
	public GameObject bomberPrefab;
	private readonly int[] evoBomberCount = new int[] { 3, 4, 5 };
	public float bomberSpeed;

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
						.MoveToTarget(enemy, towerData.evolutions[0].damage, bomberSpeed)
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
				GameObject newBomber = Instantiate(bomberPrefab, spawnPosition.transform.position, Quaternion.identity);
				currentBombers[i] = newBomber;
				HomingMissile bomberScript = newBomber.GetComponent<HomingMissile>();
				StartCoroutine(bomberScript.MoveToTarget(bomberWaitPoints[i], 0, bomberSpeed));
				break;
			}
		}
		yield return new WaitForSeconds(towerData.evolutions[0].cooldown);
		StartCoroutine(SpawnBomber());
	}

	void SkillSpawn()
	{
		GameObject newBomber = Instantiate(bomberPrefab, transform.position, Quaternion.identity, gameObject.transform);
		HomingMissile bomberScript = newBomber.GetComponent<HomingMissile>();
		bomberScript.isSkillBomber = true;
		GameObject target = TowerHelpers.SelectEnemyToAttack(
			TowerHelpers.GetEnemiesInRange(
				transform.position,
				towerData.evolutions[evolutionIndex].range,
				towerData.evolutionEnemyTypes[evolutionIndex].enemies.ToArray()
			),
			targetType
		);
		StartCoroutine(bomberScript.MoveToTarget(target, towerData.evolutions[0].damage, bomberSpeed));
	}

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	protected override IEnumerator Skill(GameObject enemy)
	{
		for (int i = 0; i < evoBomberCount[skillLevel]; i++)
		{
			SkillSpawn();
			yield return new WaitForSeconds(0.25f);
		}
		yield return null;
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }
}
