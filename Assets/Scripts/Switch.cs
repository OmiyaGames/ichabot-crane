using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Switch : MonoBehaviour {
	public bool triggerOnBody = true;
	public bool triggerOnHead = true;
	public event System.Action<bool> OnSwitchEnter;
	public event System.Action<bool> OnSwitchExit;

	void Start()
	{
		collider.isTrigger = true;
	}

	void OnTriggerEnter(Collider other)
	{
		if((enabled == true) && (OnSwitchEnter != null) && (other != null))
		{
			if((other.CompareTag("Player") == true) && (triggerOnBody == true))
			{
				OnSwitchEnter(false);
			}
			else if((other.CompareTag("Player1") == true) && (triggerOnHead == true))
			{
				OnSwitchEnter(true);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if((enabled == true) && (OnSwitchExit != null) && (other != null))
		{
			if((other.CompareTag("Player") == true) && (triggerOnBody == true))
			{
				OnSwitchExit(false);
			}
			else if((other.CompareTag("Player1") == true) && (triggerOnHead == true))
			{
				OnSwitchExit(true);
			}
		}
	}
}
