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
}
