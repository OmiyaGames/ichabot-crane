using UnityEngine;
using System.Collections;

public class ThrowHead : MonoBehaviour
{
	[SerializeField] GameObject permanentlyAttachedCamera;
	[SerializeField] Rigidbody temporarilyAttachedCamera;
	[SerializeField] float throwForce = 30;
	[SerializeField] float distanceFromBodyBeforePickingUp = 2;
	[SerializeField] GUIText pickupText;

	bool isHeadAttached = true;

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
		if(isHeadAttached == true)
		{
			if(Input.GetMouseButtonDown(0) == true)
			{
				permanentlyAttachedCamera.SetActive(false);
				temporarilyAttachedCamera.gameObject.SetActive(true);
				temporarilyAttachedCamera.transform.parent = null;
				temporarilyAttachedCamera.AddForce(permanentlyAttachedCamera.transform.forward * throwForce, ForceMode.VelocityChange);
				isHeadAttached = false;
			}
		}
		else if(Vector3.Distance(permanentlyAttachedCamera.transform.position, temporarilyAttachedCamera.transform.position) < distanceFromBodyBeforePickingUp)
		{
			if(Input.GetMouseButtonDown(0) == true)
			{
				permanentlyAttachedCamera.SetActive(true);
				temporarilyAttachedCamera.gameObject.SetActive(false);
				temporarilyAttachedCamera.transform.parent = permanentlyAttachedCamera.transform;
				temporarilyAttachedCamera.transform.localPosition = Vector3.zero;
				temporarilyAttachedCamera.transform.localRotation = Quaternion.identity;
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
