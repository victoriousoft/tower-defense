using System.Collections;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
	public bool isSkillBomber;

	public IEnumerator MoveToTarget(GameObject target, float damage, float speed)
	{
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
			// Find a new target
			GameObject[] potentialTargets = TowerHelpers.GetEnemiesInRange(
				transform.position,
				1000,
				new EnemyTypes[] { EnemyTypes.GROUND }
			);

			if (potentialTargets.Length > 0)
			{
				GameObject newTarget = potentialTargets[0];
				StartCoroutine(MoveToTarget(newTarget, damage, speed));
			}
			else
			{
				Explode(null, damage, speed);
			}
		}
		else if (target.CompareTag("Enemy"))
		{
			Explode(target, damage, speed);
		}
	}

	private void Explode(GameObject enemy, float damage, float speed)
	{
		GameObject[] enemies = TowerHelpers.GetEnemiesInRange(
			transform.position,
			0.5f,
			new EnemyTypes[] { EnemyTypes.GROUND, EnemyTypes.FLYING }
		);

		foreach (GameObject e in enemies)
		{
			if (e == null)
				break;

			float thisEnemyHealth = e.GetComponent<BaseEnemy>().health;
			e.GetComponent<BaseEnemy>().TakeDamage(damage, DamageTypes.EXPLOSION);

			if (isSkillBomber && thisEnemyHealth <= damage)
			{
				GameObject newBomber = Instantiate(gameObject, transform.position, Quaternion.identity);
				HomingMissile bomberScript = newBomber.GetComponent<HomingMissile>();
				if (bomberScript != null)
				{
					bomberScript.isSkillBomber = true;

					GameObject[] potentialTargets = TowerHelpers.GetEnemiesInRange(
						transform.position,
						100,
						new EnemyTypes[] { EnemyTypes.GROUND }
					);

					if (potentialTargets.Length > 0)
					{
						GameObject nextTarget = potentialTargets[0];
						bomberScript.MoveToTargetGetter(nextTarget, damage, speed);
					}
				}
			}
		}

		// bum efekt

		Destroy(this.gameObject);
	}

	public void MoveToTargetGetter(GameObject target, float damage, float speed)
	{
		StartCoroutine(MoveToTarget(target, damage, speed));
	}
}
