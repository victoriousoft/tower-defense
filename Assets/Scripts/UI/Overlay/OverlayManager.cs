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

	void Update()
	{
		goldText.text = "Gold: " + PlayerStatsManager.gold;
		healthText.text = "Health: " + PlayerStatsManager.lives;
		waveText.text = "Wave: " + (waveSheet.currentWave + 1) + "/" + waveSheet.waves.Length;
	}
}
