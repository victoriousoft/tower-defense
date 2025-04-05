using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
	public float speed;

	[HideInInspector]
	public float damage;
	public GameObject target;

	void Update()
	{
		if (target == null)
		{
			Destroy(gameObject);
			return;
		}

		transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

		if (Vector2.Distance(transform.position, target.transform.position) < 0.1f)
		{
			target.GetComponent<BaseTroop>().TakeDamage(damage);
			Destroy(gameObject);
		}
	}
}
