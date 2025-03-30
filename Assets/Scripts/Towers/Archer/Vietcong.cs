using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vietcong : BaseEvolutionTower
{
	private Animator flipAnimator;

	[SerializeField]
	private Transform originLeft,
		originRight;
	private Transform currentOrigin;

	[SerializeField]
	private SpriteRenderer spriteRendererLeft,
		spriteRendererRight;

	private bool facingLeft = true;

	protected override void Start()
	{
		base.Start();
		flipAnimator = GetComponent<Animator>();
		currentOrigin = originLeft;
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
		enemy.GetComponent<BaseEnemy>().TakeDamage(towerData.evolutions[1].damage, DamageTypes.PHYSICAL);
		yield return null;
	}

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	protected override IEnumerator Skill(GameObject enemy)
	{
		enemy.GetComponent<BaseEnemy>().TakeDamage(1000000, DamageTypes.PHYSICAL);
		yield return null;
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }
}
