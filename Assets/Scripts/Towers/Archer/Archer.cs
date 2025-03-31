using System.Collections;
using System.Linq;
using UnityEngine;

public class Archer : BaseTower
{
	private bool facingLeft = true;

	[SerializeField]
	private Transform originLeft,
		originRight;

	[SerializeField]
	private Transform currentOrigin;

	[SerializeField]
	private SpriteRenderer spriteRendererLeft,
		spriteRendererRight;

	public GameObject arrowPrefab;

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	protected override IEnumerator Shoot(GameObject enemy)
	{
		if (enemy != null)
		{
			if (facingLeft && enemy.transform.position.x > transform.position.x)
			{
				spriteRendererLeft.enabled = false;
				spriteRendererRight.enabled = true;
				currentOrigin = originRight;
				facingLeft = false;
			}
			else if (!facingLeft && enemy.transform.position.x < transform.position.x)
			{
				spriteRendererLeft.enabled = true;
				spriteRendererRight.enabled = false;
				currentOrigin = originLeft;
				facingLeft = true;
			}
		}
		GameObject arrow = Instantiate(arrowPrefab, currentOrigin.position, Quaternion.identity, transform);

		yield return TowerHelpers.AnimateBezierProjectile(
			arrow,
			currentOrigin.position,
			enemy,
			2,
			1,
			KillProjectile,
			TowerHelpers.TowerProjectileRotationTypes.LOOK_AT_TARGET
		);
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 _enemyPosition)
	{
		Destroy(projectile);
		if (enemy == null)
			return;

		enemy.GetComponent<BaseEnemy>().TakeDamage(towerData.levels[level].damage, DamageTypes.PHYSICAL);
	}
}
