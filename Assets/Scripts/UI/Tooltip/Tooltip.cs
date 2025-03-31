using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
	public TextMeshProUGUI headerText;
	public TextMeshProUGUI contentText;
	public LayoutElement layout;
	public RectTransform rectTransform;

	public int characterLimit;

	public void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	void Update()
	{
		UpdatePosition();
	}

	public void SetText(string header = "", string content = "")
	{
		headerText.text = header;
		contentText.text = content;

		if (string.IsNullOrEmpty(header))
		{
			headerText.gameObject.SetActive(false);
		}
		else
		{
			headerText.gameObject.SetActive(true);
		}

		if (string.IsNullOrEmpty(content))
		{
			contentText.gameObject.SetActive(false);
		}
		else
		{
			contentText.gameObject.SetActive(true);
		}
		int headerLength = headerText.text.Length;

		int contentLineLength = 0;
		foreach (string line in contentText.text.Split('\n'))
		{
			contentLineLength = Math.Max(contentLineLength, line.Length);
		}

		layout.enabled = (headerLength > characterLimit || contentLineLength > characterLimit) ? true : false;

		UpdatePosition();
	}

	private void UpdatePosition()
	{
		Vector2 position = Input.mousePosition;

		float tooltipWidth = rectTransform.rect.width * rectTransform.lossyScale.x;
		float tooltipHeight = rectTransform.rect.height * rectTransform.lossyScale.y;

		float screenWidth = Screen.width;
		float screenHeight = Screen.height;

		Vector2 offset = new Vector2(tooltipWidth / 2, 0);

		offset.x += Screen.width * 0.01f;

		if (position.x + tooltipWidth > screenWidth)
		{
			offset.x = -offset.x;
		}

		transform.position = position + offset;
	}
}
