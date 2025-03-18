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

		if (Input.GetKeyDown(KeyCode.Escape))
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

	public static void PauseGame()
	{
		isGamePaused = true;
		FullscreenOverlayManager.Show("Paused", "Press ESC to resume");
		Time.timeScale = 0;
	}

	static void ResumeGame()
	{
		isGamePaused = false;
		FullscreenOverlayManager.Hide();
		Time.timeScale = 1;
	}
}
