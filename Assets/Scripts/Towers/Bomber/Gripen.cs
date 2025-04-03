using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gripen : BaseEvolutionTower
{
	public GameObject plane;
	public float planeSpeed,
		crashDamageRadius,
		kamikadzeDamage;
	private bool onRight = true;
	private bool isFlying = false,
		crashed = false;
	public GameObject projectilePrefab;
	private bool readyToCrash = false;
	private Animator animator;
	public GameObject explosionPrefab;

	protected override void Start()
	{
		base.Start();
		animator = GetComponent<Animator>();
	}

	protected override IEnumerator Shoot(GameObject enemy)
	{
		if (!isFlying && !readyToCrash)
		{
			FlyPlane();
			yield return new WaitForSeconds(0.5f);
			for (int i = 0; i < 3; i++)
			{
				GameObject projectile = Instantiate(projectilePrefab, plane.transform.position, Quaternion.identity);
				StartCoroutine(
					projectile.GetComponent<HomingMissile>().MoveToTarget(enemy, towerData.evolutions[1].damage, 10f)
				);
				animator.SetTrigger("shoot");
				yield return new WaitForSeconds(0.33f);
			}
			yield return null;
		}
	}

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	private void FlyPlane()
	{
		if (!onRight)
		{
			animator.Play("gripen_flight_R");
			StartCoroutine(FlyFromLeftToRight());
		}
		else
		{
			animator.Play("gripen_flight_L");
			StartCoroutine(FlyFromRightToLeft());
		}
	}

	private IEnumerator FlyFromLeftToRight()
	{
		isFlying = true;

		Vector2 startPosition = Camera.main.ViewportToWorldPoint(new Vector3(0, 0));
		Vector2 endPosition = Camera.main.ViewportToWorldPoint(new Vector3(1, 0));
		startPosition = new Vector2(startPosition.x - 1.5f, transform.position.y + 2 + Random.Range(-0.75f, 0.75f));
		endPosition = new Vector2(endPosition.x + 1.5f, transform.position.y + 2 + Random.Range(-0.75f, 0.75f));

		plane.transform.position = startPosition;

		while (plane.transform.position.x < endPosition.x - 0.1f)
		{
			plane.transform.position = Vector2.MoveTowards(
				plane.transform.position,
				endPosition,
				planeSpeed * Time.deltaTime
			);
			yield return null;
		}

		yield return new WaitForSeconds(0.5f);
		onRight = true;
		isFlying = false;
	}

	private IEnumerator FlyFromRightToLeft()
	{
		isFlying = true;

		Vector2 startPosition = Camera.main.ViewportToWorldPoint(new Vector3(1, 0));
		Vector2 endPosition = Camera.main.ViewportToWorldPoint(new Vector3(0, 0));
		startPosition = new Vector2(startPosition.x + 1.5f, transform.position.y + 2 + Random.Range(-0.75f, 0.75f));
		endPosition = new Vector2(endPosition.x - 1.5f, transform.position.y + 2 + Random.Range(-0.75f, 0.75f));

		plane.transform.position = startPosition;

		while (plane.transform.position.x > endPosition.x + 0.1f)
		{
			plane.transform.position = Vector2.MoveTowards(
				plane.transform.position,
				endPosition,
				planeSpeed * Time.deltaTime
			);
			yield return null;
		}

		yield return new WaitForSeconds(0.5f);
		onRight = false;
		isFlying = false;
	}

	protected override IEnumerator Skill(GameObject enemy)
	{
		readyToCrash = true;
		onRight = true;
		yield return null;
	}

	IEnumerator Crash()
	{
		plane.transform.position = transform.position;
		animator.SetTrigger("kamikadze");

		yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.125f);

		ScreenShake.Instance.Shake(0.55f, 0.3f);
		GameObject explosionFX = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		explosionFX
			.GetComponent<SelfDestruct>()
			.DestroySelf(explosionFX.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

		GameObject[] affectedEnemies = TowerHelpers.GetEnemiesInRange(
			transform.position,
			crashDamageRadius,
			new EnemyTypes[] { EnemyTypes.GROUND, EnemyTypes.FLYING }
		);
		foreach (GameObject enemy in affectedEnemies)
		{
			enemy.GetComponent<BaseEnemy>().TakeDamage(kamikadzeDamage, DamageTypes.EXPLOSION);
		}
		readyToCrash = false;
		crashed = false;
	}

	private void LateUpdate()
	{
		if (!isFlying && readyToCrash && !crashed)
		{
			crashed = true;
			StartCoroutine(Crash());
		}
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }
}
