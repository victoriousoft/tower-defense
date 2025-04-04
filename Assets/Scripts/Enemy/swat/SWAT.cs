using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SWAT : BaseEnemy
{
	public GameObject explosionEffect;
	public float explosionDamage;

	protected override void Attack() { }

	protected override IEnumerator ExtendedDeath()
	{
		yield return new WaitForSeconds(1f);
		GameObject explosionFX = Instantiate(explosionEffect, transform.position, Quaternion.identity);
		explosionFX
			.GetComponent<SelfDestruct>()
			.DestroySelf(explosionFX.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, enemyData.stats.visRange);
		foreach (Collider2D collider in colliders)
		{
			if (collider.gameObject.CompareTag("Troop"))
				collider.gameObject.GetComponent<BaseTroop>().TakeDamage(explosionDamage);
		}
	}
}
