using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FullscreenOverlayManager : MonoBehaviour
{
	public TextMeshProUGUI titleText;
	public TextMeshProUGUI descriptionText;
	public GameObject backgroundImage;
	public GameObject menuPanel;

	private static FullscreenOverlayManager instance;

	void Start()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		Hide();
	}

	public static void Show(string title, string description)
	{
		instance.titleText.text = title;
		instance.descriptionText.text = description;
		instance.backgroundImage.SetActive(true);
		instance.menuPanel.SetActive(true);
	}

	public static void Hide()
	{
		instance.backgroundImage.SetActive(false);
		instance.menuPanel.SetActive(false);
	}
}
