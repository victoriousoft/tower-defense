using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : BaseEvolutionTower
{
	[SerializeField]
	private Animator spinAnimationAnimator;

	private float currentAngle = 0f; // Current facing angle of the tower
	private const float rotationSpeed = 100f; // Rotation speed in degrees per second

	void Update()
	{
		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 direction = mousePos - (Vector2)transform.position;

		// Calculate the target angle in degrees
		float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

		// Normalize the angles to the range [0, 360)
		currentAngle = NormalizeAngle(currentAngle);
		targetAngle = NormalizeAngle(targetAngle);

		// Determine the shortest rotation direction
		float angleDifference = targetAngle - currentAngle;
		if (angleDifference > 180)
			angleDifference -= 360;
		if (angleDifference < -180)
			angleDifference += 360;

		// Rotate toward the target angle
		if (Mathf.Abs(angleDifference) > 1f) // Allow a small tolerance to avoid jitter
		{
			if (angleDifference > 0)
			{
				if (!spinAnimationAnimator.GetCurrentAnimatorStateInfo(0).IsName("right"))
				{
					spinAnimationAnimator.SetTrigger("right");
				}
				currentAngle += Mathf.Min(rotationSpeed * Time.deltaTime, Mathf.Abs(angleDifference));
			}
			else
			{
				if (!spinAnimationAnimator.GetCurrentAnimatorStateInfo(0).IsName("left"))
				{
					spinAnimationAnimator.SetTrigger("left");
				}
				currentAngle -= Mathf.Min(rotationSpeed * Time.deltaTime, Mathf.Abs(angleDifference));
			}
		}
		else
		{
			// Stop rotation when close enough to the target
			spinAnimationAnimator.SetTrigger("idle");
		}
	}

	private float NormalizeAngle(float angle)
	{
		while (angle < 0)
			angle += 360;
		while (angle >= 360)
			angle -= 360;
		return angle;
	}

	protected override void ExtendedAwake()
	{
		spinAnimationAnimator = GetComponent<Animator>();
	}

	protected override IEnumerator Shoot(GameObject enemy)
	{
		yield return null;
	}

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }
}
