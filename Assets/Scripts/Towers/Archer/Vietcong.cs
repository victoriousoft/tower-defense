using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vietcong : BaseEvolutionTower
{
	private Animator flipAnimator;
	private Transform currentOrigin;

	[SerializeField]
	private SpriteRenderer spriteRendererLeft,
		spriteRendererRight;

	private bool facingLeft = true;

	protected override void Start()
	{
		base.Start();
		flipAnimator = GetComponent<Animator>();
		spriteRendererLeft.gameObject.SetActive(true);
		spriteRendererRight.gameObject.SetActive(false);
	}

	protected override IEnumerator Shoot(GameObject enemy)
	{
		if (enemy != null)
		{
			if (facingLeft && enemy.transform.position.x > transform.position.x)
			{
				spriteRendererLeft.gameObject.SetActive(false);
				spriteRendererRight.gameObject.SetActive(true);
				facingLeft = false;
			}
			else if (!facingLeft && enemy.transform.position.x < transform.position.x)
			{
				spriteRendererLeft.gameObject.SetActive(true);
				spriteRendererRight.gameObject.SetActive(false);
				facingLeft = true;
			}
		}
		yield return new WaitForSeconds(0.5f);
		enemy.GetComponent<BaseEnemy>().TakeDamage(towerData.evolutions[1].damage, DamageTypes.PHYSICAL);
		yield return null;
	}

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	protected override IEnumerator Skill(GameObject enemy)
	{
		//pokud closest to finish tak closest to finish, ale pokud cokoli jinyho tak most hp (check jestli nema imunitu)
		enemy.GetComponent<BaseEnemy>().TakeDamage(1000000, DamageTypes.PHYSICAL);
		yield return null;
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }
}
