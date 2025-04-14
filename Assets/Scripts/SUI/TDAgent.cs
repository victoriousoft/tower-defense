using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TowerDefenseAgent : Agent
{
	private TowerTypes[] towerTypes = new TowerTypes[4]
	{
		TowerTypes.Archer,
		TowerTypes.Barracks,
		TowerTypes.Bomb,
		TowerTypes.Magic,
	};

	private List<TowerHolderNeo> towerHolders = new List<TowerHolderNeo>();
	public static TowerDefenseAgent instance { get; private set; }

	private void Awake()
	{
		instance = this;
	}

	public override void OnEpisodeBegin()
	{
		PlayerStatsManager.ResetStats();
		Overlay.ResumeGame();
		WaveSheet.instance.Reset();

		towerHolders.Clear();

		foreach (var towerHolder in GameObject.FindObjectsOfType<BaseEnemy>())
		{
			Destroy(towerHolder.gameObject);
		}

		TowerHolderNeo[] towerHoldersArray = GameObject.FindObjectsOfType<TowerHolderNeo>();
		foreach (var towerHolder in towerHoldersArray)
		{
			towerHolder.Reset();
			towerHolders.Add(towerHolder);
		}

		Debug.Log("Episode Begin: " + towerHolders.Count + " tower holders found.");

		WaveSheet.TriggerWaveSpawn();
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		Debug.Log("Collecting observations");

		sensor.AddObservation(PlayerStatsManager.gold);
		sensor.AddObservation(PlayerStatsManager.lives);
		sensor.AddObservation(WaveSheet.instance.currentWave);
	}

	public override void OnActionReceived(ActionBuffers actions)
	{
		// buy, sell upgrade tower
		int action = actions.DiscreteActions[0];
		int towerIndex = actions.DiscreteActions[1];
		int towerType = actions.DiscreteActions[2];

		Debug.Log("Action: " + action + ", Tower Index: " + towerIndex + ", Tower Type: " + towerType);

		switch (action)
		{
			// buy tower
			case 0:
				if (towerIndex >= 0 && towerIndex < towerHolders.Count)
				{
					if (towerHolders[towerIndex].AgentBuyTower(towerTypes[towerType]))
						AddReward(0.1f);
					else
						AddReward(-0.1f);
				}
				break;

			// sell tower
			case 1:
				if (towerIndex >= 0 && towerIndex < towerHolders.Count)
				{
					towerHolders[towerIndex].SellTower();
				}
				break;

			// upgrade tower
			case 2:
				if (towerIndex >= 0 && towerIndex < towerHolders.Count)
				{
					if (towerHolders[towerIndex].AgentUpgradeTower())
						AddReward(0.1f);
					else
						AddReward(-0.1f);
				}
				break;

			default:
				Debug.LogError("Invalid action: " + action);
				break;
		}
	}

	public override void Heuristic(in ActionBuffers actionsOut)
	{
		var discreteActions = actionsOut.DiscreteActions;

		// Example heuristic: Randomly select actions for testing
		discreteActions[0] = Random.Range(0, 3); // Action: buy, sell, upgrade
		discreteActions[1] = Random.Range(0, towerHolders.Count); // Tower index
		discreteActions[2] = Random.Range(0, towerTypes.Length); // Tower type

		Debug.Log("Heuristic actions: " + discreteActions[0] + ", " + discreteActions[1] + ", " + discreteActions[2]);
	}

	public void OnEnemyPass(int livesLost)
	{
		AddReward(-0.25f * livesLost);
	}

	public void OnWaveStart(int waveIndex)
	{
		AddReward(0.5f * waveIndex);
	}

	public void GameEnd(bool win, int lives = 0)
	{
		if (win)
		{
			AddReward(lives * 0.1f);
		}
		else
		{
			AddReward(-1f);
		}

		EndEpisode();
	}
}
