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
	private static Overlay instance;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		ResumeGame();
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

	public static void Hide()
	{
		instance.gameObject.SetActive(false);
	}

	public static void Show()
	{
		instance.gameObject.SetActive(true);
	}

	public static void TogglePause()
	{
		if (isGamePaused)
			ResumeGame();
		else
			PauseGame();
	}

	public static void PauseGame(
		string title = "Paused",
		string description = "Press P to resume",
		bool[] hideButtons = null
	)
	{
		isGamePaused = true;
		hideButtons = hideButtons ?? new bool[] { false, false, false };

		FullscreenOverlayManager.Show(title, description);
		Time.timeScale = 0;
	}

	public static void ResumeGame()
	{
		FullscreenOverlayManager.Hide();
		isGamePaused = false;
		Time.timeScale = 1;
	}
}
