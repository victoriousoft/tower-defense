using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
	void Awake()
	{
		Debug.Log("MenuButton Awake");
	}

	public void ResumeClick()
	{
		Debug.Log("Resume Clicked");
	}

	public void PauseClick()
	{
		Debug.Log("Pause Clicked");
	}

	public void RestartClick()
	{
		Debug.Log("Restart Clicked");
	}

	public void MenuClick()
	{
		Debug.Log("Menu Clicked");
	}
}
