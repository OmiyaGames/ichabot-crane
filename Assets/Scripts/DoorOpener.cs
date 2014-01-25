using UnityEngine;
using System.Collections;

public class DoorOpener : MonoBehaviour
{
	public float moveUpBy = 5;
	public Switch activateSwitch;
	public bool holdToOpen = false;
	public float smoothFactor = 1;

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

	void SwitchEnter(bool isHead)
	{
		isOpen = true;
	}

	void SwitchExit(bool isHead)
	{
		if(holdToOpen == true)
		{
			isOpen = false;
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

		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, Time.deltaTime * smoothFactor);
	}
}
