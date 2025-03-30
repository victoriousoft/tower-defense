using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatsManager : MonoBehaviour
{
	public static int currentLevel = -1;
	public static int lives = 20;
	public static int gold = 999999999;

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
		Overlay.PauseGame("Game Over", "press f5 to restart (I'm a lazy developer)");
	}

	public static void WinGame()
	{
		int stars = 0;

		if (lives <= 10)
			stars = 1;
		else if (lives <= 15)
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
		ResetStats();
	}

	public static void ReturnToMenu()
	{
		currentLevel = -1;
		lives = 20;
		gold = 999999999;
		SceneManager.LoadScene("Scenes/Levels/MainMenu");
	}
}
