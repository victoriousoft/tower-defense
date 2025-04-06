using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WaveTriggerButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	public HealthBar statusBar;
	public bool isMouseOver = false;

	void Awake()
	{
		statusBar.gameObject.SetActive(false);
	}

	void Update()
	{
		if (isMouseOver)
		{
			SetNextWaveButtonState();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isMouseOver = true;
		SetNextWaveButtonState();
		WaveSheet.instance.waves[WaveSheet.instance.currentWave + 1].DrawPaths();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		isMouseOver = false;
		PlayerStatsManager.AddGold(
			WaveSheet.instance.waves[WaveSheet.instance.currentWave + 1].GetEarlyCallCashback(1 - statusBar.progress)
		);
		WaveSheet.instance.waves[WaveSheet.instance.currentWave + 1].HidePaths();
		WaveSheet.TriggerWaveSpawn();
		TooltipManager.Hide();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isMouseOver = false;
		TooltipManager.Hide();
		WaveSheet.instance.waves[WaveSheet.instance.currentWave + 1].HidePaths();
	}

	private void SetNextWaveButtonState()
	{
		WaveSheet.Wave nextWave = WaveSheet.instance.waves[WaveSheet.instance.currentWave + 1];

		string cashbackTitle = "";
		string cashbackText = "";

		if (WaveSheet.instance.currentWave != -1)
		{
			cashbackTitle = "Call next wave early";
			cashbackText =
				"\nFor Early call you will receive " + nextWave.GetEarlyCallCashback(1 - statusBar.progress) + " gold";
		}
		else
		{
			cashbackTitle = "Call first wave";
		}

		TooltipManager.Show(cashbackTitle, nextWave.GetWaveInfo() + cashbackText);
	}
}
