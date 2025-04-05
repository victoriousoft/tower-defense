using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatsManager : MonoBehaviour
{
#if UNITY_EDITOR
	public static int currentLevel = 0;
#else
	public static int currentLevel = -1;
#endif
	public static int currentWave = 0;
	public static int lives = 25;
	public static int gold = 260;

	[System.NonSerialized]
	[HideInInspector]
	public static List<int> levelStars = new List<int>();

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

	public static void ResetStats()
	{
		lives = GlobalData.instance.levelSheet.levels[currentLevel].initialLives;
		gold = GlobalData.instance.levelSheet.levels[currentLevel].initialGold;
		levelStars = new List<int>();
	}

	public static void GameOver()
	{
		Overlay.PauseGame("Game Over", "You have lost all your lives");
	}

	public static void WinGame()
	{
		int initialLives = GlobalData.instance.levelSheet.levels[currentLevel].initialLives;
		int stars = 0;

		float percentRemaining = (float)lives / initialLives;

		if (percentRemaining <= 0.5f)
			stars = 1;
		else if (percentRemaining <= 0.75f)
			stars = 2;
		else
			stars = 3;

		Overlay.PauseGame("You Win, " + stars + " stars", "wait for the save to upload, then go back to the main menu");
		WebGLMessageHandler.SendToJavaScript(
			new WebGLMessageHandler.OutBrowserMessage
			{
				action = "levelPass",
				args = new { level = currentLevel, stars = stars },
			}
		);
	}
}
