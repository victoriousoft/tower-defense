using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
	public static ScreenShake Instance;
	private Vector3 originalPosition;
	private bool isShaking = false;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
		originalPosition = transform.position;
	}

	public void Shake(float duration, float magnitude)
	{
		if (!isShaking)
		{
			StartCoroutine(ShakeCoroutine(duration, magnitude));
		}
	}

	private IEnumerator ShakeCoroutine(float duration, float magnitude)
	{
		isShaking = true;
		float elapsed = 0f;

		while (elapsed < duration)
		{
			float xOffset = Random.Range(-1f, 1f) * magnitude;
			float yOffset = Random.Range(-1f, 1f) * magnitude;

			transform.position = originalPosition + new Vector3(xOffset, yOffset, 0);
			elapsed += Time.deltaTime;
			yield return null;
		}

		// Smoothly return to the original position
		float returnSpeed = 5f;
		while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
		{
			transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * returnSpeed);
			yield return null;
		}

		transform.position = originalPosition; // Final snap back
		isShaking = false;
	}
}
