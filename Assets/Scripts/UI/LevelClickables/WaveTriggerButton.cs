using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WaveTriggerButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	public HealthBar statusBar;

	public void Awake()
	{
		statusBar.gameObject.SetActive(false);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		WaveSheet.Wave nextWave = WaveSheet.instance.waves[WaveSheet.instance.currentWave + 1];

		string cashbackText = "";
		if (WaveSheet.instance.currentWave != -1)
		{
			cashbackText =
				"\nFor Early call you will receive " + nextWave.GetEarlyCallCashback(statusBar.progress) + " gold";
		}

		TooltipManager.Show("Call next wave early", nextWave.GetWaveInfo() + cashbackText);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		WaveSheet.TriggerWaveSpawn();
		TooltipManager.Hide();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		TooltipManager.Hide();
	}
}
