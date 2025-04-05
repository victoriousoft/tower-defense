using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
	public static void PlaySound(AudioSource source, AudioClip clip, bool loop = false)
	{
		source.clip = clip;
		source.loop = loop;
		source.volume = GlobalData.instance.volume / 100f;
		source.Play();
	}

	public static GameObject PlayInBackground(GameObject sourceGameObject, AudioClip clip, bool loop = false)
	{
		if (clip == null)
		{
			return null;
		}

		GameObject soundObject = new("SoundPlayer - " + clip.name);
		soundObject.transform.SetParent(sourceGameObject.transform);

		AudioSource source = soundObject.AddComponent<AudioSource>();

		source.clip = clip;
		source.loop = loop;
		source.volume = GlobalData.instance.volume / 100f;
		source.Play();

		if (!loop)
		{
			Destroy(soundObject, clip.length);
		}

		return soundObject;
	}

	public static GameObject PlayInBackgroundRandom(GameObject sourceGameObject, AudioClip[] clips, bool loop = false)
	{
		if (clips == null || clips.Length == 0)
		{
			return null;
		}

		int randomIndex = Random.Range(0, clips.Length);
		return PlayInBackground(sourceGameObject, clips[randomIndex], loop);
	}
}
