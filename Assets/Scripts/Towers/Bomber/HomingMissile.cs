using System.Collections;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
	public bool isSkillBomber;
	private Animator animator;
	private bool movable = true;

	void Start()
	{
		animator = GetComponent<Animator>();
	}

	public IEnumerator MoveToTarget(GameObject target, float damage, float speed)
	{
		while (target != null && Vector3.Distance(transform.position, target.transform.position) > 0.1f && movable)
		{
			float direction = target.transform.position.x - transform.position.x;
			animator.SetFloat("x", direction);

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

		if (enemies.Length == 0)
		{
			Destroy(this.gameObject);
			return;
		}

		foreach (GameObject e in enemies)
		{
			if (e == null)
				continue;

			e.GetComponent<BaseEnemy>().TakeDamage(damage, DamageTypes.EXPLOSION);

			if (isSkillBomber && e.GetComponent<BaseEnemy>().health <= 0)
			{
				GameObject[] potentialTargets = TowerHelpers.GetEnemiesInRange(
					transform.position,
					100,
					new EnemyTypes[] { EnemyTypes.GROUND }
				);

				if (potentialTargets.Length > 0)
				{
					GameObject newBomber = Instantiate(gameObject, transform.position, Quaternion.identity);
					HomingMissile bomberScript = newBomber.GetComponent<HomingMissile>();
					if (bomberScript != null)
					{
						bomberScript.isSkillBomber = true;
						GameObject nextTarget = potentialTargets[0];
						bomberScript.MoveToTargetGetter(nextTarget, damage, speed);
					}
				}
			}
		}

		// bum efekt
		animator.SetTrigger("explode");
		movable = false;
		Destroy(this.gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
	}

	public void MoveToTargetGetter(GameObject target, float damage, float speed)
	{
		StartCoroutine(MoveToTarget(target, damage, speed));
	}
}
