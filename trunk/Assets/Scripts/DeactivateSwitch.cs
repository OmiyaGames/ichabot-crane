using UnityEngine;
using System.Collections;

public class DeactivateSwitch : MonoBehaviour
{
	public Switch switchToDeactivate;

	// Use this for initialization
	void Start () {
		switchToDeactivate.OnSwitchEnter += OnSwitchEnter;
	}
	
	// Update is called once per frame
	void OnSwitchEnter (Collider isHead)
	{
		StartCoroutine(DelayDeactivate());
	}

	IEnumerator DelayDeactivate()
	{
		yield return null;
		switchToDeactivate.IsTriggerEnabled = false;
	}
}
