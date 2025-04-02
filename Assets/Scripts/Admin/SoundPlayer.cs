using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
	public static void PlaySound(AudioSource source, AudioClip clip, bool loop = false)
	{
		source.clip = clip;
		source.loop = loop;
		source.Play();
	}
}
