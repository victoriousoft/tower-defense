using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
	public Tooltip tooltip;

	private static TooltipManager instance;

	void Awake()
	{
		instance = this;
		Hide();
	}

	public static void Show(string header = "", string content = "")
	{
		instance.tooltip.SetText(header, content);
		instance.tooltip.gameObject.SetActive(true);
	}

	public static void Hide()
	{
		instance.tooltip.gameObject.SetActive(false);
	}
}
