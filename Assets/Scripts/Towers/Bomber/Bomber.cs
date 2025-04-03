using System.Collections;
using UnityEngine;

public class Bomber : BaseTower
{
	public float splashRadius = 1;
	public Transform shotOrigin;
	public GameObject bombPrefab,
		explosionPrefab;

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	protected override IEnumerator Shoot(GameObject enemy)
	{
		GameObject bomb = Instantiate(bombPrefab);

		yield return TowerHelpers.AnimateBezierProjectile(
			bomb,
			shotOrigin.position,
			enemy,
			4,
			0.75f,
			KillProjectile,
			TowerHelpers.TowerProjectileRotationTypes.SPIN
		);
	}

	protected override void KillProjectile(GameObject bomb, GameObject enemy, Vector3 enemyPosition)
	{
		Destroy(bomb);
		GameObject explosionFX = Instantiate(explosionPrefab, enemyPosition, Quaternion.identity);
		explosionFX
			.GetComponent<SelfDestruct>()
			.DestroySelf(explosionFX.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
		GameObject[] enemies = TowerHelpers.GetEnemiesInRange(enemyPosition, splashRadius, towerData.enemyTypes);
		foreach (GameObject e in enemies)
		{
			if (e == null)
				return;
			e.GetComponent<BaseEnemy>().TakeDamage(towerData.levels[level].damage, DamageTypes.EXPLOSION);
		}
	}
}
