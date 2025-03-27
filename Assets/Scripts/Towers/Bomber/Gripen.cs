using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gripen : BaseEvolutionTower
{
	public GameObject plane;
	public float planeSpeed = 10f;
	private bool onRight = true;
	private bool isFlying = false;
	public GameObject projectilePrefab;
	private bool readyToCrash = false;
	public GameObject ExplosionEffect;

	protected override void Start()
	{
		base.Start();
	}

	protected override IEnumerator Shoot(GameObject enemy)
	{
		FlyPlane();
		yield return new WaitForSeconds(0.5f);
		for (int i = 0; i < 3; i++)
		{
			GameObject projectile = Instantiate(projectilePrefab, plane.transform.position, Quaternion.identity);
			StartCoroutine(
				projectile.GetComponent<HomingMissile>().MoveToTarget(enemy, towerData.evolutions[1].damage, 10f)
			);
			yield return new WaitForSeconds(0.33f);
		}
		yield return null;
	}

	protected override IEnumerator ChargeUp(GameObject enemy)
	{
		yield return null;
	}

	private void FlyPlane()
	{
		if (!isFlying && !readyToCrash)
		{
			if (!onRight)
			{
				StartCoroutine(FlyFromLeftToRight());
			}
			else
			{
				StartCoroutine(FlyFromRightToLeft());
			}
		}
		else if (readyToCrash)
		{
			Crash();
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

		while (plane.transform.position.x < endPosition.x)
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

		while (plane.transform.position.x > endPosition.x)
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
		yield return null;
	}

	IEnumerator Crash()
	{
		//annimace padu + vybuch + zniceni
		yield return new WaitForSeconds(0.1f);
	}

	protected override void KillProjectile(GameObject projectile, GameObject enemy, Vector3 enemyPosition) { }
}
