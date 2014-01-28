using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class DoorOpener : MonoBehaviour
{
	public float moveUpBy = 5;
	public Switch activateSwitch;
	public bool holdToOpen = false;
	public float smoothFactor = 1;
	public AudioClip openSound;
	public AudioClip closeSound;

	bool isOpen = false;
	Vector3 originalPosition;
	Vector3 openPosition;
	Vector3 velocity;

	// Use this for initialization
	void Start ()
	{
		activateSwitch.OnSwitchEnter += SwitchEnter;
		activateSwitch.OnSwitchExit += SwitchExit;
		originalPosition = transform.position;
		openPosition = originalPosition;
		openPosition += transform.up * moveUpBy;
	}

	void SwitchEnter(Collider isHead)
	{
		isOpen = true;
		audio.PlayOneShot(openSound);
	}

	void SwitchExit(Collider isHead)
	{
		if(holdToOpen == true)
		{
			isOpen = false;
			audio.PlayOneShot(closeSound);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 targetPosition = originalPosition;
		if(isOpen == true)
		{
			targetPosition = openPosition;
		}

		rigidbody.MovePosition(Vector3.SmoothDamp(rigidbody.position, targetPosition, ref velocity, Time.deltaTime * smoothFactor));
	}
}
