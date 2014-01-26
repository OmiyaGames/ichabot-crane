using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class Booster : MonoBehaviour
{
	public float bodyLaunchVelocity = 500;
	public float headLaunchVelocity = 50;
	Rigidbody playerBody = null;
	Rigidbody playerHead = null;
	int index = 0;

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") == true)
		{
			playerBody = other.GetComponent<Rigidbody>();
		}
		else if(other.CompareTag("Player1") == true)
		{
			playerHead = other.GetComponent<Rigidbody>();
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Player") == true)
		{
			playerBody = null;
		}
		else if(other.CompareTag("Player1") == true)
		{
			playerHead = null;
		}
	}

	void FixedUpdate()
	{
		if(playerBody != null)
		{
			playerBody.AddForce(transform.up * bodyLaunchVelocity);
		}
		if(playerHead != null)
		{
			playerHead.velocity = transform.up * headLaunchVelocity;
		}
	}
}
