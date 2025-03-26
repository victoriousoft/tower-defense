using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SUIcideBomber : MonoBehaviour
{
	public IEnumerator MoveToTarget(GameObject target, float damage)
	{
		while (target != null && Vector3.Distance(transform.position, target.transform.position) > 0.1f)
		{
			transform.position = Vector3.MoveTowards(
				transform.position,
				target.transform.position,
				Time.deltaTime * 5f
			);
			yield return null;
		}

		if (target == null)
		{
			//najit jinyho
			if (
				TowerHelpers.GetEnemiesInRange(transform.position, 1000, new EnemyTypes[] { EnemyTypes.GROUND }).Length
				> 0
			)
			{
				GameObject newTarget = TowerHelpers.GetEnemiesInRange(
					transform.position,
					1000,
					new EnemyTypes[] { EnemyTypes.GROUND }
				)[0];
				StartCoroutine(MoveToTarget(newTarget, damage));
			}
			else
			{
				Explode(null, 0);
			}
		}
		else if (target.CompareTag("Enemy"))
		{
			Explode(target, damage);
		}
	}

	private void Explode(GameObject enemy, float damage)
	{
		if (enemy != null)
			enemy.GetComponent<BaseEnemy>().TakeDamage(damage, DamageTypes.EXPLOSION);
		//bum anim
		Destroy(gameObject);
	}
}
