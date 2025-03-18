using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FullscreenOverlayManager : MonoBehaviour
{
	public TextMeshProUGUI titleText;
	public TextMeshProUGUI descriptionText;

	private static FullscreenOverlayManager instance;

	void Start()
	{
		instance = this;
		Hide();
	}

	public static void Show(string title, string description)
	{
		instance.titleText.text = title;
		instance.descriptionText.text = description;
		instance.gameObject.SetActive(true);
	}

	public static void Hide()
	{
		instance.gameObject.SetActive(false);
	}
}
