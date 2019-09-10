using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundOnCollision : MonoBehaviour
{
	void OnCollisionEnter()
	{
		GetComponent<AudioSource>().Stop();
		GetComponent<AudioSource>().Play();
	}
}
