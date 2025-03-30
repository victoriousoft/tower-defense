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
	private bool isFlying = false;
	public GameObject projectilePrefab;
	private bool readyToCrash = false;
	public GameObject ExplosionEffect;
	private Animator animator;

	protected override void Start()
	{
		base.Start();
		animator = GetComponent<Animator>();
	}

	protected override IEnumerator Shoot(GameObject enemy)
	{
		Debug.Log(readyToCrash);
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
		else if (readyToCrash)
		{
			Debug.Log("Crash");
			StartCoroutine(Crash());
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
		startPosition = new Vector2(startPosition.x - 1, transform.position.y + 2 + Random.Range(-0.75f, 0.75f));
		endPosition = new Vector2(endPosition.x + 1, transform.position.y + 2 + Random.Range(-0.75f, 0.75f));

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

		onRight = true;
		isFlying = false;
	}

	private IEnumerator FlyFromRightToLeft()
	{
		isFlying = true;

		Vector2 startPosition = Camera.main.ViewportToWorldPoint(new Vector3(1, 0));
		Vector2 endPosition = Camera.main.ViewportToWorldPoint(new Vector3(0, 0));
		startPosition = new Vector2(startPosition.x + 1, transform.position.y + 2 + Random.Range(-0.75f, 0.75f));
		endPosition = new Vector2(endPosition.x - 1, transform.position.y + 2 + Random.Range(-0.75f, 0.75f));

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
		Debug.Log("Crash");
		plane.transform.position = transform.position;
		animator.SetTrigger("kamikadze");
		//annimace padu + vybuch + zniceni
		GameObject[] affectedEnemies = TowerHelpers.GetEnemiesInRange(
			transform.position,
			towerData.evolutions[0].range,
			towerData.enemyTypes
		);
		foreach (GameObject enemy in affectedEnemies)
		{
			enemy.GetComponent<BaseEnemy>().TakeDamage(kamikadzeDamage, DamageTypes.EXPLOSION);
		}
		yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 1);
		readyToCrash = false;
	}

	private void LateUpdate()
	{
		if (!isFlying && readyToCrash)
			StartCoroutine(Crash());
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }
}
