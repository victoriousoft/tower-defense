using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
	public static int lives = 20;
	public static int gold = 999999999;

	public static void AddGold(int value)
	{
		gold += value;
	}

	public static bool SubtractGold(int value)
	{
		if (gold >= value)
		{
			gold -= value;
			return true;
		}
		else
			return false;
	}

	public static void SubtractLives(int value)
	{
		lives -= value;
		if (lives <= 0)
			GameOver();
	}

	static void GameOver()
	{
		Debug.Log("take the L");
	}
}
