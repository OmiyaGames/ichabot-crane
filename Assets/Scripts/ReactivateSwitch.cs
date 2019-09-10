using UnityEngine;
using System.Collections;

public class ReactivateSwitch : MonoBehaviour
{
	public Switch switchToReactivate;

	// Use this for initialization
	void Start () {
		switchToReactivate.OnSwitchEnter += OnSwitchEnter;
	}
	
	// Update is called once per frame
	void OnSwitchEnter (Collider isHead)
	{
		StartCoroutine(DelayReactivate());
	}

	IEnumerator DelayReactivate()
	{
		yield return null;
		switchToReactivate.IsTriggerEnabled = true;
	}
}
