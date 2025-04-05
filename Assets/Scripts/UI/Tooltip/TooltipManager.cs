using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
	public Tooltip tooltip;

	private static TooltipManager instance;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}

		Hide();
	}

	public static void Show(string header = "", string content = "")
	{
		Show(header, content, Color.white);
	}

	public static void Show(string header, string content, Color textColor)
	{
		instance.tooltip.headerText.color = textColor;

		instance.tooltip.SetText(header, content);
		instance.tooltip.gameObject.SetActive(true);
	}

	public static void Hide()
	{
		instance.tooltip.gameObject.SetActive(false);
	}
}
