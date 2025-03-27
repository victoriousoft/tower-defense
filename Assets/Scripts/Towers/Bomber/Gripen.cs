using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gripen : BaseEvolutionTower
{
	EnemyTypes[] targetEnemyTypes = new EnemyTypes[] { EnemyTypes.GROUND };

	protected override void Start()
	{
		base.Start();
	}

	protected override IEnumerator Shoot(GameObject enemy)
	{
		yield return null;
	}

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }
}
