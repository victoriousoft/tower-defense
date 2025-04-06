using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyNote : BaseEnemy
{
	public GameObject sprite;
	public float attackSpeed;
	private Coroutine attackRoutine;

	void Start() { }

	protected override void Attack()
	{
		if (attackRoutine == null)
			attackRoutine = StartCoroutine(MoveAttack());
	}

	private IEnumerator MoveAttack()
	{
		while (Vector3.Distance(sprite.transform.position, currentTarget.transform.position) > 0.5f)
		{
			animator.SetBool("idle", true);
			transform.position = Vector3.MoveTowards(
				transform.position,
				currentTarget.transform.position,
				Time.fixedDeltaTime * attackSpeed
			);
			yield return null;
		}
		currentTarget.GetComponent<BaseTroop>().TakeDamage(1000);
		Destroy(gameObject);
	}

	protected override void UseAbility() { }
}
