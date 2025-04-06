using System;
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

		public string GetWaveInfo()
		{
			string info = "";

			foreach (WaveEnemy enemy in enemies)
			{
				info += enemy.count + "x " + enemy.enemyPrefab.name + "\n";
			}

			return info;
		}

		public int GetEarlyCallCashback(float progress)
		{
			int secondsRemaining = Mathf.CeilToInt(initialDelay * (1 - progress));
			int cashback = Mathf.CeilToInt(secondsRemaining * 3);
			return cashback;
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

			yield return new WaitForSecondsRealtime(initialDelay);

			for (int i = 0; i < count; i++)
			{
				GameObject enemy = Instantiate(enemyPrefab);
				enemy.GetComponent<BaseEnemy>().SetPathParent(pathParent);
				enemy.transform.SetParent(GameObject.Find("Enemies").transform);

				if (i < count - 1)
				{
					yield return new WaitForSecondsRealtime(spawnDelay);
				}
			}
		}
	}

	public WaveTriggerButton waveTriggerButton;

	[SerializeField]
	public Wave[] waves;

	[System.NonSerialized]
	[HideInInspector]
	public int currentWave = -1;

	[System.NonSerialized]
	[HideInInspector]
	public bool showNextWaveButton = true;

	public AudioClip waveStartSound;

	private Coroutine waveCountdownRoutine;

	public static WaveSheet instance;

	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Debug.LogError("WaveSheet instance already exists, destroying this one.");
			Destroy(gameObject);
			return;
		}
	}

	public static void TriggerWaveSpawn()
	{
		if (instance.waveCountdownRoutine != null)
		{
			instance.StopCoroutine(instance.waveCountdownRoutine);
			instance.waveCountdownRoutine = null;
		}

		SoundPlayer.PlayInBackground(instance.gameObject, instance.waveStartSound);

		instance.waveTriggerButton.gameObject.SetActive(false);
		instance.StartCoroutine(instance.SpawnWave(instance.currentWave + 1));
	}

	private IEnumerator AwaitAllEnemyDeath()
	{
		while (GameObject.Find("Enemies").transform.childCount > 0)
		{
			yield return new WaitForSecondsRealtime(0.5f);
		}

		PlayerStatsManager.WinGame();
	}

	public IEnumerator SpawnWave(int waveIndex)
	{
		if (waveTriggerButton.GetComponent<WaveTriggerButton>().isMouseOver && TooltipManager.instance.isEnabled)
		{
			TooltipManager.Hide();
		}

		if (waveIndex >= waves.Length)
		{
			Debug.LogError(
				"Wave index out of range, requested wave index: " + waveIndex + ", total waves: " + waves.Length
			);
			yield break;
		}

		currentWave = waveIndex;

		yield return StartCoroutine(waves[waveIndex].SpawnWave(this));

		if (waveIndex + 1 < waves.Length)
		{
			waveTriggerButton.gameObject.SetActive(true);
			waveTriggerButton.statusBar.gameObject.SetActive(true);

			waveCountdownRoutine = StartCoroutine(
				waveTriggerButton.statusBar.Animate(
					1,
					0,
					waves[waveIndex + 1].initialDelay,
					() =>
					{
						waveTriggerButton.gameObject.SetActive(false);
						waveTriggerButton.statusBar.gameObject.SetActive(false);
						waveCountdownRoutine = null;
						TriggerWaveSpawn();
					}
				)
			);

			yield return waveCountdownRoutine;
		}
		else
		{
			StartCoroutine(AwaitAllEnemyDeath());
		}
	}
}
