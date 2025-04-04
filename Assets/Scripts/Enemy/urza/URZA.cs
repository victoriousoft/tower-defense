using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URZA : BaseEnemy
{
	public int catCount;
	public GameObject catPrefab,
		projectilePrefab;

	protected override void Attack()
	{
		StartCoroutine(Shoot());
	}

	protected override void UseAbility()
	{
		StartCoroutine(SpawnCats(catCount, false));
	}

	private IEnumerator Shoot()
	{
		if (currentTarget == null)
			yield break;
		yield return new WaitForSeconds(0.5f);
		animator.SetTrigger("attack");

		GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
		EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
		projectileScript.damage = currentDamage;
		projectileScript.target = currentTarget;
	}

	private IEnumerator SpawnCats(int catCount, bool buffed)
	{
		for (int i = 0; i < catCount; i++)
		{
			StartCoroutine(
				SpawnChild(
					catPrefab,
					Random.Range(-0.3f, 0.3f),
					child =>
					{
						if (buffed)
						{
							child.GetComponent<BaseEnemy>().currentDamage = 1000;
						}
					}
				)
			);
			yield return new WaitForSeconds(0.75f);
			animator.Play("ability");
		}

		abilityInUse = false;
		animator.Play("moveRight");
	}

	protected override IEnumerator ExtendedDeath()
	{
		yield return new WaitForSeconds(1f);
		SpawnCats(1, true);
	}
}
