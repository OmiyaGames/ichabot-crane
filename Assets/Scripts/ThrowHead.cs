using UnityEngine;
using System.Collections;

public class ThrowHead : MonoBehaviour
{
	[SerializeField] GameObject permanentlyAttachedCamera;
	[SerializeField] SimpleMouseRotator temporarilyAttachedCamera;
	[SerializeField] float throwForce = 30;
	[SerializeField] float distanceFromBodyBeforePickingUp = 2;
	[SerializeField] SimpleMouseRotator[] allMouseRotationScripts;
	[SerializeField] GUIText pickupText;

	bool isHeadAttached = true;
	bool isFireButtonDown = false;

	public bool IsHeadAttached
	{
		get
		{
			return isHeadAttached;
		}
	}

	void Start()
	{
		pickupText.enabled = false;
	}

	// Update is called once per frame
	void Update ()
	{
		// Check to see if fire has been clicked
		bool fireButtonPressed = false;
		if(Input.GetAxis("Fire1").CompareTo(0) != 0)
		{
			if(isFireButtonDown == false)
			{
				fireButtonPressed = true;
				isFireButtonDown = true;
			}
		}
		else
		{
			isFireButtonDown = false;
		}

		// Check if head is attached
		if(isHeadAttached == true)
		{
			if(fireButtonPressed == true)
			{
				permanentlyAttachedCamera.SetActive(false);
				temporarilyAttachedCamera.gameObject.SetActive(true);
				temporarilyAttachedCamera.transform.parent = null;
				temporarilyAttachedCamera.rigidbody.AddForce(permanentlyAttachedCamera.transform.forward * throwForce, ForceMode.VelocityChange);
				temporarilyAttachedCamera.Start();
				foreach(SimpleMouseRotator rotator in allMouseRotationScripts)
				{
					rotator.enabled = false;
				}
				isHeadAttached = false;
			}
		}
		else if(Vector3.Distance(permanentlyAttachedCamera.transform.position, temporarilyAttachedCamera.transform.position) < distanceFromBodyBeforePickingUp)
		{
			if(fireButtonPressed == true)
			{
				permanentlyAttachedCamera.SetActive(true);
				temporarilyAttachedCamera.gameObject.SetActive(false);
				temporarilyAttachedCamera.transform.parent = permanentlyAttachedCamera.transform;
				temporarilyAttachedCamera.transform.localPosition = Vector3.zero;
				temporarilyAttachedCamera.transform.localRotation = Quaternion.identity;
				foreach(SimpleMouseRotator rotator in allMouseRotationScripts)
				{
					rotator.enabled = false;
				}
				pickupText.enabled = false;
				isHeadAttached = true;
			}
			else
			{
				pickupText.enabled = true;
			}
		}
		else
		{
			pickupText.enabled = false;
		}
	}
}
