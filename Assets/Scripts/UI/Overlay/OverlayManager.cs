using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Overlay : MonoBehaviour
{
	public TextMeshProUGUI goldText;
	public TextMeshProUGUI healthText;
	public TextMeshProUGUI waveText;

	public WaveSheet waveSheet;

	private static bool isGamePaused = false;
	private Overlay instance;

	void Start()
	{
		instance = this;
	}

	void Update()
	{
		goldText.text = "Gold: " + PlayerStatsManager.gold;
		healthText.text = "Health: " + PlayerStatsManager.lives;
		waveText.text = "Wave: " + (waveSheet.currentWave + 1) + "/" + waveSheet.waves.Length;

		if (Input.GetKeyDown(KeyCode.P))
		{
			TogglePause();
		}
	}

	public static void TogglePause()
	{
		if (isGamePaused)
			ResumeGame();
		else
			PauseGame();
	}

	public static void PauseGame(string title = "Paused", string description = "Press P to resume")
	{
		isGamePaused = true;
		FullscreenOverlayManager.Show(title, description);
		Time.timeScale = 0;
	}

	public static void ResumeGame()
	{
		isGamePaused = false;
		FullscreenOverlayManager.Hide();
		Time.timeScale = 1;
	}
}
