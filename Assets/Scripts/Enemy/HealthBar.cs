using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	public Image foregroundImage;

	public void SetHealth(float healthNormalized)
	{
		foregroundImage.fillAmount = healthNormalized;
	}

	public IEnumerator Animate(float start, float end, float duration)
	{
		transform.gameObject.SetActive(true);
		float elapsed = 0f;

		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			foregroundImage.fillAmount = Mathf.Lerp(start, end, elapsed / duration);
			yield return null;
		}

		foregroundImage.fillAmount = end;
		transform.gameObject.SetActive(false);
	}
}
