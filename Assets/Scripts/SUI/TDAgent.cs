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
	private HashSet<int> visitedTowerIndices = new HashSet<int>();
	private Dictionary<int, int> actionFrequency = new Dictionary<int, int>();
	private int consecutiveDoNothing = 0;

	private void FixedUpdate()
	{
		timeSinceLevelLoad += Time.fixedDeltaTime;
		timeSinceLastAction += Time.fixedDeltaTime;

		// Small constant penalty for inactivity
		AddReward(-0.0005f);

		// Punish the agent if it has been idle for too long
		if (timeSinceLastAction > idleThreshold)
		{
			AddReward(-2f); // Increased penalty for inactivity
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
		// Reset player stats and game state
		PlayerStatsManager.ResetStats();
		Overlay.ResumeGame();
		WaveSheet.instance.Reset();
		timeSinceLevelLoad = 0f;

		// Clear and reset tower holders
		towerHolders.Clear();
		visitedTowerIndices.Clear();
		actionFrequency.Clear();

		// Destroy all enemies
		foreach (var enemy in GameObject.FindObjectsOfType<BaseEnemy>())
		{
			Destroy(enemy.gameObject);
		}

		// Initialize tower holders
		TowerHolderNeo[] holdersArray = GameObject.FindObjectsOfType<TowerHolderNeo>();
		foreach (var holder in holdersArray)
		{
			holder.Reset();
			towerHolders.Add(holder);
		}

		Debug.Log("Episode Begin: " + towerHolders.Count + " tower holders found.");

		// Trigger the first wave
		WaveSheet.TriggerWaveSpawn();
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		// Add player stats and game state as observations
		sensor.AddObservation(PlayerStatsManager.gold);
		sensor.AddObservation(PlayerStatsManager.lives);
		sensor.AddObservation(WaveSheet.instance.currentWave);
		sensor.AddObservation(timeSinceLevelLoad);

		// Add tower holder states as observations
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

		// Determine available actions based on tower holder states
		for (int i = 0; i < towerHolders.Count; i++)
		{
			var holder = towerHolders[i];
			var tower = holder.towerInstance;
			int towerLevel = tower != null ? tower.GetComponent<BaseTower>().level : -1;

			bool isIdle = holder.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");

			if (!isIdle)
			{
				continue; // Skip if the holder is busy
			}

			if (tower == null)
			{
				canBuy = true; // Empty spot → can buy
			}
			else
			{
				canSell = true; // Tower exists → can sell

				if (towerLevel < 2)
				{
					canUpgrade = true; // Tower not maxed → can upgrade
				}
			}
		}

		// Mask unavailable actions
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

		// Reset idle tracking if the agent performs an action
		if (action != 3) // Not "Do Nothing"
		{
			timeSinceLastAction = 0f;
			consecutiveDoNothing = 0;
		}
		else
		{
			consecutiveDoNothing++;
		}

		// Log the action for debugging
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

		Debug.Log($"Action: {actionString}, Tower Index: {towerIndex}, Tower Type: {towerTypeString}");

		// Track action frequency and punish repetitive actions
		if (!actionFrequency.ContainsKey(action))
		{
			actionFrequency[action] = 0;
		}
		actionFrequency[action]++;

		if (actionFrequency[action] > 5)
		{
			AddReward(-0.1f);
		}

		// Reward visiting new tower indices
		if (!visitedTowerIndices.Contains(towerIndex))
		{
			AddReward(0.5f);
			visitedTowerIndices.Add(towerIndex);
		}

		// Handle actions
		switch (action)
		{
			case 0: // Buy
				if (
					IsValidTowerHolder(towerIndex)
					&& towerHolders[towerIndex].animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")
				)
				{
					if (towerHolders[towerIndex].AgentBuyTower(towerTypes[towerType]))
					{
						AddReward(0.4f);
					}
					else
					{
						AddReward(-0.6f);
					}
				}
				else
				{
					AddReward(-1f);
				}
				break;

			case 1: // Sell
				if (IsValidTowerHolder(towerIndex))
				{
					if (towerHolders[towerIndex].towerInstance != null)
					{
						towerHolders[towerIndex].SellTower();
						AddReward(0f);
					}
					else
					{
						AddReward(-0.4f);
					}
				}
				else
				{
					AddReward(-1f);
				}
				break;

			case 2: // Upgrade
				if (IsValidTowerHolder(towerIndex))
				{
					if (towerHolders[towerIndex].AgentUpgradeTower())
					{
						AddReward(0.5f);
					}
					else
					{
						AddReward(-0.6f);
					}
				}
				else
				{
					AddReward(-1f);
				}
				break;

			case 3: // Do Nothing
				AddReward(-0.02f); // Small constant penalty
				if (consecutiveDoNothing > 5)
				{
					Debug.LogWarning($"Doing nothing for {consecutiveDoNothing} frames.");
					AddReward(-0.1f * (consecutiveDoNothing - 5)); // Punish after 5 consecutive idles
				}
				break;

			default:
				AddReward(-2f); // Invalid action penalty
				break;
		}
	}

	public override void Heuristic(in ActionBuffers actionsOut)
	{
		var discreteActions = actionsOut.DiscreteActions;

		// Randomize actions for heuristic mode
		discreteActions[0] = Random.Range(0, 4); // Action type
		discreteActions[1] = Random.Range(0, towerHolders.Count); // Tower index
		discreteActions[2] = Random.Range(0, towerTypes.Length); // Tower type

		Debug.Log($"Heuristic: {discreteActions[0]}, {discreteActions[1]}, {discreteActions[2]}");
	}

	public void OnEnemyPass(int livesLost)
	{
		Debug.Log($"Enemy passed! Lives lost: {livesLost}");
		AddReward(-5f * livesLost); // Punish for losing lives
	}

	public void OnWaveStart(int waveIndex)
	{
		Debug.Log($"Wave {waveIndex} started.");
		AddReward(0.2f * waveIndex); // Reward for starting a new wave
	}

	public void OnEnemyDeath(int enemyLives)
	{
		Debug.Log($"Enemy killed worth {enemyLives} lives");
		AddReward(0.5f * enemyLives); // Reward for killing enemies
	}

	public void GameEnd(bool win, int lives = 0)
	{
		Debug.Log($"Game Ended. Win: {win}, Lives: {lives}");

		if (win)
		{
			AddReward(2f + (lives * 0.2f)); // Reward for winning
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
