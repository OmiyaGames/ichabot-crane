using UnityEngine;
using System.Collections;

public class ThrowHead : MonoBehaviour
{
	[SerializeField] GameObject permanentlyAttachedCamera;
	[SerializeField] SimpleMouseRotator temporarilyAttachedCamera;
	[SerializeField] HopHead headHopper;
	[SerializeField] float throwForce = 30;
	[SerializeField] float distanceFromBodyBeforePickingUp = 2;
	[SerializeField] bool enableHeadTossing = true;
	SimpleMouseRotator[] allMouseRotationScripts;

	bool isHeadAttached = true;
	bool isFireButtonDown = false;
	static bool isPaused = false;

	public bool IsHeadAttached
	{
		get
		{
			return isHeadAttached;
		}
	}

	public Transform HeadTransform
	{
		get
		{
			return temporarilyAttachedCamera.transform;
		}
	}

	public static bool IsPaused
	{
		get
		{
			return isPaused;
		}
		set
		{
			isPaused = value;
		}
	}

	void Start()
	{
		allMouseRotationScripts = GetComponentsInChildren<SimpleMouseRotator>();
		isPaused = false;
		Screen.lockCursor = true;
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

		if(Input.GetKey(KeyCode.Escape) == true)
		{
			isPaused = true;
			Time.timeScale = 0;
			Screen.lockCursor = false;
		}

		// Check if head is attached
		if(isHeadAttached == true)
		{
			if((enableHeadTossing == true) && (fireButtonPressed == true))
			{
				permanentlyAttachedCamera.SetActive(false);
				temporarilyAttachedCamera.gameObject.SetActive(true);
				temporarilyAttachedCamera.transform.parent = null;
				temporarilyAttachedCamera.rigidbody.velocity = rigidbody.velocity;
				temporarilyAttachedCamera.rigidbody.AddForce(permanentlyAttachedCamera.transform.forward * throwForce, ForceMode.VelocityChange);
				temporarilyAttachedCamera.Start();
				foreach(SimpleMouseRotator rotator in allMouseRotationScripts)
				{
					rotator.enabled = false;
				}
				headHopper.TossHead();
				isHeadAttached = false;
			}
		}
		else if(Vector3.Distance(permanentlyAttachedCamera.transform.position, temporarilyAttachedCamera.transform.position) < distanceFromBodyBeforePickingUp)
		{
			PauseMenu.ShowMessage("Click to pick up head");
			if(fireButtonPressed == true)
			{
				permanentlyAttachedCamera.SetActive(true);
				temporarilyAttachedCamera.gameObject.SetActive(false);
				temporarilyAttachedCamera.transform.parent = permanentlyAttachedCamera.transform;
				temporarilyAttachedCamera.transform.localPosition = Vector3.zero;
				temporarilyAttachedCamera.transform.localRotation = Quaternion.identity;
				foreach(SimpleMouseRotator rotator in allMouseRotationScripts)
				{
					rotator.enabled = true;
				}
				isHeadAttached = true;
				PauseMenu.HideMessage();
			}
		}
		else
		{
			PauseMenu.HideMessage();
			if(fireButtonPressed == true)
			{
				headHopper.Jump();
			}
		}
	}
}
