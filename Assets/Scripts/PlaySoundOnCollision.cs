using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundOnCollision : MonoBehaviour
{
	void OnCollisionEnter()
	{
		audio.Stop();
		audio.Play();
	}
}
