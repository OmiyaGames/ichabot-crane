using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class Booster : MonoBehaviour
{
	public float bodyLaunchVelocity = 500;
	public float headLaunchVelocity = 50;
	Rigidbody playerBody = null;
	Rigidbody playerHead = null;

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") == true)
		{
			playerBody = other.GetComponent<Rigidbody>();
			audio.Stop();
			audio.Play();
		}
		else if(other.CompareTag("Player1") == true)
		{
			playerHead = other.GetComponent<Rigidbody>();
			audio.Stop();
			audio.Play();
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
		if((playerBody != null) && (playerBody.isKinematic == false))
		{
			playerBody.AddForce(transform.up * bodyLaunchVelocity);
		}
		if((playerHead != null) && (playerHead.isKinematic == false))
		{
			playerHead.velocity = transform.up * headLaunchVelocity;
		}
	}
}
