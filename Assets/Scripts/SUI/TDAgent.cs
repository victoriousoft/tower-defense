using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

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
	private float timeSinceLevelLoad = 0f;

	private float timeSinceLastAction = 0f;
	private const float idleThreshold = 10f; // Threshold in seconds for punishing inactivity

	private void FixedUpdate()
	{
		timeSinceLevelLoad += Time.fixedDeltaTime;
		timeSinceLastAction += Time.fixedDeltaTime;

		// Small constant penalty for inactivity
		AddReward(-0.001f);

		// Punish the agent if it has been idle for too long
		if (timeSinceLastAction > idleThreshold)
		{
			AddReward(-1f); // Apply a penalty for inactivity
			Debug.LogWarning("Agent punished for inactivity (timeout).");

			timeSinceLastAction -= idleThreshold; // Subtract threshold instead of resetting to 0
		}
	}

	private void Awake()
	{
		base.Awake();
		instance = this;
	}

	public override void OnEpisodeBegin()
	{
		PlayerStatsManager.ResetStats();
		Overlay.ResumeGame();
		WaveSheet.instance.Reset();
		timeSinceLevelLoad = 0f;

		towerHolders.Clear();

		foreach (var enemy in GameObject.FindObjectsOfType<BaseEnemy>())
		{
			Destroy(enemy.gameObject);
		}

		TowerHolderNeo[] holdersArray = GameObject.FindObjectsOfType<TowerHolderNeo>();
		foreach (var holder in holdersArray)
		{
			holder.Reset();
			towerHolders.Add(holder);
		}

		Debug.Log("Episode Begin: " + towerHolders.Count + " tower holders found.");

		WaveSheet.TriggerWaveSpawn();
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		sensor.AddObservation(PlayerStatsManager.gold);
		sensor.AddObservation(PlayerStatsManager.lives);
		sensor.AddObservation(WaveSheet.instance.currentWave);
		sensor.AddObservation(timeSinceLevelLoad);

		foreach (var holder in towerHolders)
		{
			sensor.AddObservation(
				holder.towerInstance != null ? holder.towerInstance.GetComponent<BaseTower>().level : -1
			);
		}
	}

	public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
	{
		bool canBuy = false;
		bool canSell = false;
		bool canUpgrade = false;

		for (int i = 0; i < towerHolders.Count; i++)
		{
			var holder = towerHolders[i];
			var tower = holder.towerInstance;
			int towerLevel = tower != null ? tower.GetComponent<BaseTower>().level : -1;

			bool isIdle = holder.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");

			if (!isIdle)
			{
				continue; // if busy, can't interact
			}

			if (tower == null)
			{
				canBuy = true; // empty spot → can buy
			}
			else
			{
				canSell = true; // there's a tower → can sell

				if (towerLevel < 2)
				{
					canUpgrade = true; // tower not maxed → can upgrade
				}
			}
		}

		// Now mask based on global possibilities:
		if (!canBuy)
		{
			actionMask.SetActionEnabled(0, 0, false); // Buy
		}
		if (!canSell)
		{
			actionMask.SetActionEnabled(0, 1, false); // Sell
		}
		if (!canUpgrade)
		{
			actionMask.SetActionEnabled(0, 2, false); // Upgrade
		}
	}

	public override void OnActionReceived(ActionBuffers actions)
	{
		int action = actions.DiscreteActions[0];
		int towerIndex = actions.DiscreteActions[1];
		int towerType = actions.DiscreteActions[2];

		if (action != 3) // not "Do Nothing"
		{
			timeSinceLastAction = 0f;
		}

		string actionString = action switch
		{
			0 => "Buy",
			1 => "Sell",
			2 => "Upgrade",
			3 => "Do Nothing",
			_ => "Invalid Action",
		};
		string towerTypeString = towerType switch
		{
			0 => "Archer",
			1 => "Barracks",
			2 => "Bomb",
			3 => "Magic",
			_ => "Invalid Tower Type",
		};

		Debug.Log($"Action received: {actionString}, Tower Index: {towerIndex}, Tower Type: {towerTypeString}");

		switch (action)
		{
			case 0: // Buy tower
				if (
					IsValidTowerHolder(towerIndex)
					&& towerHolders[towerIndex].animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")
				)
				{
					if (towerHolders[towerIndex].AgentBuyTower(towerTypes[towerType]))
					{
						AddReward(0.5f); // Reward for buying early
						Debug.Log($"Bought {towerTypes[towerType]} at {towerIndex}");
					}
					else
					{
						AddReward(-0.5f); // Buying failed (e.g., not enough gold)
					}
				}
				else
				{
					AddReward(-1f); // Invalid buy attempt
				}
				break;

			case 1: // Sell tower
				if (IsValidTowerHolder(towerIndex))
				{
					if (towerHolders[towerIndex].towerInstance != null)
					{
						towerHolders[towerIndex].SellTower();
						AddReward(0.3f); // Small reward for managing resources
						Debug.Log($"Sold tower at {towerIndex}");
					}
					else
					{
						AddReward(-0.3f); // Tried to sell nothing
					}
				}
				else
				{
					AddReward(-1f); // Invalid sell attempt
				}
				break;

			case 2: // Upgrade tower
				if (IsValidTowerHolder(towerIndex))
				{
					if (towerHolders[towerIndex].AgentUpgradeTower())
					{
						AddReward(0.6f); // Reward for improving defenses
						Debug.Log($"Upgraded tower at {towerIndex}");
					}
					else
					{
						AddReward(-0.5f); // Upgrade failed
					}
				}
				else
				{
					AddReward(-1f); // Invalid upgrade attempt
				}
				break;

			case 3: // Do nothing
				AddReward(-0.01f);
				break;

			default:
				Debug.LogError($"Invalid action received: {action}");
				AddReward(-2f); // Strong punishment for illegal action
				break;
		}
	}

	public override void Heuristic(in ActionBuffers actionsOut)
	{
		var discreteActions = actionsOut.DiscreteActions;

		discreteActions[0] = Random.Range(0, 3); // Buy, Sell, Upgrade
		discreteActions[1] = Random.Range(0, towerHolders.Count); // Tower index
		discreteActions[2] = Random.Range(0, towerTypes.Length); // Tower type

		Debug.Log($"Heuristic: {discreteActions[0]}, {discreteActions[1]}, {discreteActions[2]}");
	}

	public void OnEnemyPass(int livesLost)
	{
		Debug.Log($"Enemy passed! Lives lost: {livesLost}");
		AddReward(-5f * livesLost);
	}

	public void OnWaveStart(int waveIndex)
	{
		Debug.Log($"Wave {waveIndex} started.");
		AddReward(0.2f * waveIndex);
	}

	public void OnEnemyDeath(int enemyLives)
	{
		Debug.Log($"Enemy killed worth {enemyLives} lives");
		AddReward(0.5f * enemyLives);
	}

	public void GameEnd(bool win, int lives = 0)
	{
		Debug.Log($"Game Ended. Win: {win}, Lives: {lives}");

		if (win)
		{
			AddReward(2f + (lives * 0.2f)); // Bigger win reward
		}
		else
		{
			AddReward(-2f); // Punishment for losing
		}

		EndEpisode();
	}

	private bool IsValidTowerHolder(int index)
	{
		return index >= 0 && index < towerHolders.Count;
	}
}
