using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	public Image foregroundImage;

	// accept a float value between 0 and 1
	public void SetHealth(float healthNormalized)
	{
		foregroundImage.fillAmount = healthNormalized;
	}

	// accept a float value between 0 and 1
	public IEnumerator Animate(float start, float end, float durationInSeconds, Action callback = null)
	{
		float elapsed = 0f;

		while (elapsed < durationInSeconds)
		{
			elapsed += Time.deltaTime;
			foregroundImage.fillAmount = Mathf.Lerp(start, end, elapsed / durationInSeconds);
			yield return null;
		}

		foregroundImage.fillAmount = end;

		callback?.Invoke();
	}
}
