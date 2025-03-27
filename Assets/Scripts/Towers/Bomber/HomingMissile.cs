using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
	public IEnumerator MoveToTarget(GameObject target, float damage, float speed)
	{
		Debug.Log("Moving to target");
		while (target != null && Vector3.Distance(transform.position, target.transform.position) > 0.1f)
		{
			transform.position = Vector3.MoveTowards(
				transform.position,
				target.transform.position,
				Time.deltaTime * speed
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
				StartCoroutine(MoveToTarget(newTarget, damage, speed));
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
