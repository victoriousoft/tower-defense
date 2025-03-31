using System;
using System.Collections;
using UnityEngine;

public class MachineGun : BaseEvolutionTower
{
	[SerializeField]
	private Animator spinAnimationAnimator;

	private int currentHeading = 135;
	private int targetHeading = 135;
	private Coroutine rotationCoroutine;
	private GameObject currentEnemy;
	private bool skillInUse = false;

	protected override void Start()
	{
		base.Start();

		spinAnimationAnimator = GetComponent<Animator>();
	}

	void Update()
	{
		if (skillInUse)
			return;
		if (currentEnemy != null && rotationCoroutine == null)
		{
			Vector2 directionToEnemy = (currentEnemy.transform.position - transform.position).normalized;
			float angle = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x) * Mathf.Rad2Deg;
			angle = (90 - angle) % 360;
			if (angle < 0)
				angle += 360;

			targetHeading = Mathf.RoundToInt(angle / 45f) * 45;
			targetHeading = NormalizeHeading(targetHeading);

			int angleDifference = targetHeading - currentHeading;

			if (angleDifference > 180)
				angleDifference -= 360;
			else if (angleDifference < -180)
				angleDifference += 360;

			if (angleDifference != 0)
			{
				rotationCoroutine = StartCoroutine(RotateStepByStep(angleDifference));
			}
		}
		else if (currentEnemy == null && rotationCoroutine != null)
		{
			int angleTo135 = 135 - currentHeading;
			if (angleTo135 > 180)
				angleTo135 -= 360;
			else if (angleTo135 < -180)
				angleTo135 += 360;

			rotationCoroutine = StartCoroutine(RotateStepByStep(angleTo135));
		}
		spinAnimationAnimator.SetBool("connected", currentHeading == targetHeading && currentEnemy != null);
	}

	private IEnumerator RotateStepByStep(int angleDifference)
	{
		if (rotationCoroutine != null)
			yield break;

		// Determine the shortest rotation path
		if (angleDifference > 180)
			angleDifference -= 360;
		else if (angleDifference < -180)
			angleDifference += 360;

		while (currentHeading != targetHeading && currentEnemy != null && !skillInUse)
		{
			int newHeading;
			if (angleDifference > 0)
			{
				newHeading = currentHeading + 45;
				spinAnimationAnimator.SetTrigger("right");
			}
			else
			{
				newHeading = currentHeading - 45;
				spinAnimationAnimator.SetTrigger("left");
			}

			currentHeading = NormalizeHeading(newHeading);

			angleDifference = targetHeading - currentHeading;
			if (angleDifference > 180)
				angleDifference -= 360;
			else if (angleDifference < -180)
				angleDifference += 360;

			yield return new WaitForSeconds(0.1f);

			if (currentEnemy == null || skillInUse)
			{
				rotationCoroutine = null;
				yield break;
			}
		}

		rotationCoroutine = null;
		currentEnemy = null;
	}

	protected override IEnumerator Shoot(GameObject enemy)
	{
		currentEnemy = enemy;

		if (skillInUse || rotationCoroutine != null || targetHeading != currentHeading)
		{
			spinAnimationAnimator.SetTrigger("idle");
			yield break;
		}

		enemy.GetComponent<BaseEnemy>().TakeDamage(towerData.evolutions[0].damage, DamageTypes.PHYSICAL);

		if (enemy == null)
		{
			currentEnemy = null;
			yield break;
		}

		yield return null;
	}

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	protected override IEnumerator Skill(GameObject enemy)
	{
		skillInUse = true;
		currentEnemy = null;
		spinAnimationAnimator.speed = 3f;

		spinAnimationAnimator.SetTrigger("attack");

		for (int i = 0; i < 360 / 45 * 10; i++)
		{
			GameObject[] enemies = TowerHelpers.GetEnemiesInRange(
				transform.position,
				towerData.evolutions[0].range,
				towerData.enemyTypes
			);

			spinAnimationAnimator.SetTrigger("right");
			currentHeading = NormalizeHeading(currentHeading + 45);
			targetHeading = currentHeading;

			if (enemies.Length > 0)
			{
				StartCoroutine(DatageEnemies(enemies));
			}

			yield return new WaitForSeconds(
				Mathf.Max(spinAnimationAnimator.GetCurrentAnimatorStateInfo(0).length + 0.01f, 0.025f)
			);
		}

		skillInUse = false;
		spinAnimationAnimator.speed = 1f;

		yield return null;
	}

	private IEnumerator DatageEnemies(GameObject[] enemies)
	{
		foreach (GameObject enemy in enemies)
		{
			if (enemy != null)
				enemy.GetComponent<BaseEnemy>().TakeDamage(towerData.evolutions[0].damage, DamageTypes.PHYSICAL);
		}

		yield return null;
	}

	private int NormalizeHeading(int heading)
	{
		if (heading < 0)
			return 360 + heading;
		else if (heading >= 360)
			return heading - 360;
		else
			return heading;
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }
}
