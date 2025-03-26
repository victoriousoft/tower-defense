using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleEastBomber : BaseEvolutionTower
{
	EnemyTypes[] targetEnemyTypes = new EnemyTypes[] { EnemyTypes.GROUND };

	protected override void Start()
	{
		base.Start();
	}

	void Update()
	{
		//check jestli neni v blizkosti holy order a pokud ano, tak zvysit damage
	}

	protected override IEnumerator Shoot(GameObject enemy)
	{
		//spawn s-bomber
		yield return null;
	}

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }
}
