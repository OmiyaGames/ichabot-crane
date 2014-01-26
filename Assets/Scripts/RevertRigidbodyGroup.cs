using UnityEngine;
using System.Collections;

public class RevertRigidbodyGroup : MonoBehaviour
{
	public Switch activateSwitch;
	public float explosionForce = 10;
	public float explosionRadius = 50;
	bool isTriggered = false;

	// Use this for initialization
	void Start ()
	{
		activateSwitch.OnSwitchEnter += SwitchEnter;
	}
	
	void SwitchEnter(Collider head)
	{
		if(isTriggered == false)
		{
			isTriggered = true;
			Rigidbody[] allRigidbodies = GetComponentsInChildren<Rigidbody>();
			foreach(Rigidbody body in allRigidbodies)
			{
				body.isKinematic = false;
				body.AddExplosionForce(explosionForce, head.transform.position, explosionRadius);
			}
		}
	}
}
