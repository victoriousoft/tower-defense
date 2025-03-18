using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTriggerButton : MonoBehaviour
{
	public WaveSheet waveSheet;
	public HealthBar statusBar;

	public void Awake()
	{
		statusBar.gameObject.SetActive(false);
	}

	void OnMouseDown()
	{
		waveSheet.GetComponent<WaveSheet>().TriggerWaveSpawn();
		TooltipManager.Hide();
	}

	void OnMouseEnter()
	{
		WaveSheet.Wave nextWave = waveSheet.waves[waveSheet.currentWave + 1];

		string cashbackText = "";
		if (waveSheet.currentWave != -1)
		{
			cashbackText =
				"\nFor Early call you will receive " + nextWave.GetEarlyCallCashback(statusBar.progress) + " gold";
		}

		TooltipManager.Show("Call next wave early", nextWave.GetWaveInfo() + cashbackText);
	}

	void OnMouseExit()
	{
		TooltipManager.Hide();
	}
}
