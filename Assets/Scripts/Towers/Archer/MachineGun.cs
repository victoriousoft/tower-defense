using System.Collections;
using UnityEngine;

public class MachineGun : BaseEvolutionTower
{
	[SerializeField]
	private Animator spinAnimationAnimator;

	private float currentAngle = 135f;
	private float targetAngle;
	private Coroutine rotationCoroutine;
	private GameObject currentEnemy;
	private bool skillInUse = false;

	protected override void Start()
	{
		base.Start();

		targetAngle = currentAngle;
		spinAnimationAnimator = GetComponent<Animator>();
	}

	void Update()
	{
		if (currentEnemy != null)
		{
			Vector2 direction = (Vector2)currentEnemy.transform.position - (Vector2)transform.position;

			targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 180;

			currentAngle = NormalizeAngle(currentAngle);
			targetAngle = NormalizeAngle(targetAngle);

			if (rotationCoroutine == null && Mathf.Abs(targetAngle - currentAngle) > 22.5f)
			{
				rotationCoroutine = StartCoroutine(RotateStepByStep());
			}
		}
	}

	private IEnumerator RotateStepByStep()
	{
		if (currentEnemy == null || skillInUse)
		{
			rotationCoroutine = null;
			yield break;
		}
		yield return new WaitForSeconds(0.1f);
		while (Mathf.Abs(targetAngle - currentAngle) > 22.5f)
		{
			float angleDifference = targetAngle - currentAngle;
			if (angleDifference > 180)
				angleDifference -= 360;
			if (angleDifference < -180)
				angleDifference += 360;

			if (angleDifference > 0)
			{
				currentAngle += 45f;
				spinAnimationAnimator.SetTrigger("left");
			}
			else
			{
				currentAngle -= 45f;
				spinAnimationAnimator.SetTrigger("right");
			}
			currentAngle = NormalizeAngle(currentAngle);

			yield return new WaitForSeconds(0.1f);
		}
		rotationCoroutine = StartCoroutine(RotateStepByStep());
	}

	private float NormalizeAngle(float angle)
	{
		while (angle < 0)
			angle += 360;
		while (angle > 360)
			angle -= 360;
		return angle;
	}

	protected override IEnumerator Shoot(GameObject enemy)
	{
		if (skillInUse)
			yield break;
		currentEnemy = enemy;
		enemy.GetComponent<BaseEnemy>().TakeDamage(towerData.evolutions[0].damage, DamageTypes.PHYSICAL);
		yield return null;
	}

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	protected override IEnumerator Skill(GameObject enemy)
	{
		skillInUse = true;
		spinAnimationAnimator.speed = 3f;
		GameObject[] enemies = TowerHelpers.GetEnemiesInRange(
			transform.position,
			towerData.evolutions[0].range,
			towerData.enemyTypes
		);
		for (int i = 0; i < 96; i++)
		{
			spinAnimationAnimator.SetTrigger("right");
			foreach (GameObject targetEnemy in enemies)
			{
				if (targetEnemy != null)
					targetEnemy
						.GetComponent<BaseEnemy>()
						.TakeDamage(towerData.evolutions[0].damage, DamageTypes.PHYSICAL);
			}
			yield return new WaitForSeconds(spinAnimationAnimator.GetCurrentAnimatorStateInfo(0).length);
		}
		skillInUse = false;
		spinAnimationAnimator.speed = 1f;
		yield return null;
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }
}
