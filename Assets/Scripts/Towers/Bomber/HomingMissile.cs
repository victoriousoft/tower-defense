using System.Collections;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
	public bool isSkillBomber;
	private Animator animator;
	private bool movable = true;
	public GameObject explosionPrefab;
	private GameObject currentTarget;

	void Awake()
	{
		animator = GetComponent<Animator>();
	}

	public IEnumerator MoveToTarget(GameObject target, float damage, float speed)
	{
		currentTarget = target;
		float startTime = Time.time;

		while (target != null && Vector3.Distance(transform.position, target.transform.position) > 0.1f && movable)
		{
			if (animator != null)
				animator.SetFloat("x", target.transform.position.x - transform.position.x);

			transform.position = Vector3.MoveTowards(
				transform.position,
				target.transform.position,
				Time.deltaTime * speed
			);

			yield return null;
		}

		if (damage == 0)
			yield break;
		if (target == null)
		{
			FindNewTargetAndMove(damage, speed);
		}
		else
		{
			StartCoroutine(Explode(target, damage, speed));
		}
	}

	private IEnumerator Explode(GameObject enemy, float damage, float speed)
	{
		GameObject[] enemies = TowerHelpers.GetEnemiesInRange(
			transform.position,
			0.5f,
			new EnemyTypes[] { EnemyTypes.GROUND, EnemyTypes.FLYING }
		);

		if (enemies.Length == 0)
		{
			Destroy(this.gameObject);
			yield break;
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

		if (animator != null)
			animator.SetTrigger("explode");
		movable = false;
		if (animator != null)
			yield return new WaitForSeconds(0.25f);
		GameObject explosionFX = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		explosionFX
			.GetComponent<SelfDestruct>()
			.DestroySelf(explosionFX.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
		Destroy(this.gameObject);
	}

	void FindNewTargetAndMove(float damage, float speed)
	{
		GameObject[] potentialTargets = TowerHelpers.GetEnemiesInRange(
			transform.position,
			1000,
			new EnemyTypes[] { EnemyTypes.GROUND }
		);

		GameObject newTarget = null;
		foreach (GameObject enemy in potentialTargets)
		{
			if (enemy != null && enemy != currentTarget)
			{
				newTarget = enemy;
				break;
			}
		}

		if (newTarget != null)
		{
			StartCoroutine(MoveToTarget(newTarget, damage, speed));
		}
		else
		{
			StartCoroutine(Explode(null, damage, speed));
		}
	}

	public void MoveToTargetGetter(GameObject target, float damage, float speed)
	{
		StartCoroutine(MoveToTarget(target, damage, speed));
	}
}
