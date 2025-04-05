using System.Collections;
using UnityEngine;

public class DogBox : BaseEnemy
{
	public GameObject[] childPrefabs;
	public int spawnCount;

	protected override void Attack() { }

	protected override IEnumerator ExtendedDeath()
	{
		for (int i = 0; i < spawnCount; i++)
		{
			Debug.Log("DogBox: ExtendedDeath() called");
			StartCoroutine(SpawnChild(GetRandomChildPrefab(), Random.Range(-0.5f, 0.5f), null));
			yield return new WaitForSeconds(0.1f);
		}
	}

	private GameObject GetRandomChildPrefab()
	{
		float[] weights = new float[] { 0.3f, 0.3f, 0.3f, 0.1f };

		float totalWeight = 0f;
		foreach (float weight in weights)
		{
			totalWeight += weight;
		}

		float randomValue = Random.Range(0f, totalWeight);

		float cumulativeWeight = 0f;
		for (int i = 0; i < childPrefabs.Length; i++)
		{
			cumulativeWeight += weights[i];
			if (randomValue <= cumulativeWeight)
			{
				return childPrefabs[i];
			}
		}

		return childPrefabs[0];
	}
}
