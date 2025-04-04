using System.Collections;
using UnityEngine;

public class HealDogBox : BaseEnemy
{
	public float healAmount = 10f;
	public float healRange = 5f;
	public float healInterval = 3f;

	private LineRenderer lineRenderer;

	void Start()
	{
		lineRenderer = gameObject.GetComponent<LineRenderer>();
		lineRenderer.positionCount = 2;
		lineRenderer.enabled = false;

		StartCoroutine(HealLoop());
	}

	protected override void Attack() { }

	IEnumerator HealLoop()
	{
		while (true)
		{
			yield return new WaitForSeconds(healInterval);

			GameObject target = FindDamagedAlly();
			if (target != null)
			{
				BaseEnemy ally = target.GetComponent<BaseEnemy>();
				if (ally != null)
				{
					ally.Heal(healAmount);
					StartCoroutine(ShowHealLine(target.transform));
				}
			}
		}
	}

	GameObject FindDamagedAlly()
	{
		GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject obj in allEnemies)
		{
			if (obj == this.gameObject)
				continue;

			BaseEnemy other = obj.GetComponent<BaseEnemy>();
			if (other != null && other.health < other.enemyData.stats.maxHealth)
			{
				float distance = Vector2.Distance(transform.position, obj.transform.position);
				if (distance <= healRange)
				{
					return obj;
				}
			}
		}
		return null;
	}

	IEnumerator ShowHealLine(Transform target)
	{
		lineRenderer.enabled = true;
		float duration = 0.5f;
		float timer = 0f;

		while (timer < duration)
		{
			if (target != null)
			{
				lineRenderer.SetPosition(0, transform.position);
				lineRenderer.SetPosition(1, target.position);
			}
			timer += Time.deltaTime;
			yield return null;
		}

		lineRenderer.enabled = false;
	}
}
