using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaveSheet : MonoBehaviour
{
	[System.Serializable]
	public class Wave
	{
		public float initialDelay; // in seconds, delay before the first spawn
		public WaveEnemy[] enemies;

		public IEnumerator SpawnWave(MonoBehaviour instance)
		{
			List<Coroutine> spawnRoutines = new List<Coroutine>();

			foreach (WaveEnemy enemy in enemies)
			{
				spawnRoutines.Add(instance.StartCoroutine(enemy.SpawnWaveEnemy()));
			}

			foreach (Coroutine routine in spawnRoutines)
			{
				yield return routine;
			}

			yield return null;
		}
	}

	[System.Serializable]
	public class WaveEnemy
	{
		public GameObject enemyPrefab;
		public Transform pathParent;
		public int count;
		public float spawnDelay; // in seconds, delay between each spawn
		public float initialDelay; // in seconds, delay before the first spawn

		public IEnumerator SpawnWaveEnemy()
		{
			yield return null;

			yield return new WaitForSeconds(initialDelay);

			for (int i = 0; i < count; i++)
			{
				GameObject enemy = Instantiate(enemyPrefab);
				enemy.GetComponent<BaseEnemy>().SetPathParent(pathParent);
				enemy.transform.SetParent(GameObject.Find("Enemies").transform);

				if (i < count - 1)
				{
					yield return new WaitForSeconds(spawnDelay);
				}
			}
		}
	}

	[SerializeField]
	public Wave[] waves;

	[System.NonSerialized]
	[HideInInspector]
	public int currentWave = -1;

	public void Awake()
	{
		StartCoroutine(SpawnWaves());
	}

	public IEnumerator SpawnWaves(int startWaveOverride = 0)
	{
		yield return new WaitForSeconds(waves[0].initialDelay);
		currentWave++;

		for (int i = startWaveOverride; i < waves.Length; i++)
		{
			Wave wave = waves[i];

			yield return StartCoroutine(wave.SpawnWave(this));

			if (i < waves.Length - 1)
			{
				yield return new WaitForSeconds(waves[i + 1].initialDelay);
				currentWave++;
			}
		}
	}
}
