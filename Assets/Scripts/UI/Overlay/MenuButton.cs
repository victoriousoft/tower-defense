using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
	public void ResumeClick()
	{
		Overlay.TogglePause();
	}

	public void PauseClick()
	{
		Overlay.PauseGame();
	}

	public void RestartClick()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void MenuClick()
	{
		SceneManager.LoadScene("Menu/MainMenu");
	}
}
