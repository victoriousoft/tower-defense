using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URZA : BaseEnemy
{
	public int catCount;
	public GameObject catPrefab;

	protected override void Attack() { }

	protected override void UseAbility()
	{
		//animace
		StartCoroutine(SpawnCats(catCount, false));
	}

	private IEnumerator SpawnCats(int catCount, bool buffed)
	{
		for (int i = 0; i < catCount; i++)
		{
			GameObject cat = catPrefab;
			SpawnChild(cat, Random.Range(-0.3f, 0.3f));
			cat.transform.SetParent(transform);
			if (buffed)
			{
				cat.GetComponent<BaseEnemy>().currentDamage = 1000;
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	protected override IEnumerator ExtendedDeath()
	{
		yield return new WaitForSeconds(1f);
		SpawnCats(1, true);
	}
}
