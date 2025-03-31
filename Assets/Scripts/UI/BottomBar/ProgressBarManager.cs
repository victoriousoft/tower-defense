using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarManager : MonoBehaviour
{
	[SerializeField]
	public GameObject[] progressBars;

	public void SetProgress(int count)
	{
		count = Mathf.Min(count, progressBars.Length);

		for (int i = 0; i < progressBars.Length; i++)
		{
			ProgressBar bar = progressBars[i].GetComponent<ProgressBar>();

			if (i < count)
			{
				bar.background.SetActive(false);
				bar.foreground.SetActive(true);
			}
			else
			{
				bar.background.SetActive(true);
				bar.foreground.SetActive(false);
			}
		}
	}
}
