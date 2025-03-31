using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
	public void ResumeClick()
	{
		Overlay.ResumeGame();
	}

	public void PauseClick()
	{
		Overlay.PauseGame();
	}

	public void RestartClick()
	{
		PlayerStatsManager.ResetStats();
		Overlay.ResumeGame();
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void MenuClick()
	{
		FullscreenOverlayManager.Hide();
		SceneManager.LoadScene(GlobalData.instance.levelSheet.mainMenuSceneName);
	}

	public void HideBottomBarClick()
	{
		BottomBar.Hide();
	}
}
