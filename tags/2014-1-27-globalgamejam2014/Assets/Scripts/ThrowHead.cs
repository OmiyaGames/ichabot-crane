using UnityEngine;
using System.Collections;

[RequireComponent(typeof(FirstPersonCharacter))]
[RequireComponent(typeof(AudioSource))]
public class ThrowHead : MonoBehaviour
{
	public const float AnimationSnapFactor = 0.1f;
	[SerializeField] GameObject permanentlyAttachedCamera;
	[SerializeField] SimpleMouseRotator temporarilyAttachedCamera;
	[SerializeField] HopHead headHopper;
	[SerializeField] float throwForce = 30;
	[SerializeField] float distanceFromBodyBeforePickingUp = 2;
	[SerializeField] bool enableHeadTossing = true;
	[SerializeField] float headAttachementLerpFactor = 1;
	[SerializeField] float headRotationLerpFactor = 1;
	[SerializeField] AudioClip throwHeadSound;
	[SerializeField] AudioClip pickupHeadSound;
	SimpleMouseRotator[] allMouseRotationScripts;
	FirstPersonCharacter controller;
	bool isHeadAttached = true;
	bool isFireButtonDown = false;
	bool isAnimatingHeadAttachement = false;
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
		controller = GetComponent<FirstPersonCharacter>();
		isPaused = false;
		isAnimatingHeadAttachement = false;
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
			AimReticle.Reticle.enabled = false;
		}

		// Check if head is attached
		if(isHeadAttached == true)
		{
			if(isAnimatingHeadAttachement == true)
			{
				if(Vector3.Distance(temporarilyAttachedCamera.transform.position, permanentlyAttachedCamera.transform.position) < AnimationSnapFactor)
				{
					controller.isControlsEnabled = true;
					isAnimatingHeadAttachement = false;
					temporarilyAttachedCamera.transform.parent = permanentlyAttachedCamera.transform;
					permanentlyAttachedCamera.SetActive(true);
					temporarilyAttachedCamera.gameObject.SetActive(false);
					temporarilyAttachedCamera.transform.localPosition = Vector3.zero;
					temporarilyAttachedCamera.transform.localRotation = Quaternion.identity;
					foreach(SimpleMouseRotator rotator in allMouseRotationScripts)
					{
						rotator.enabled = true;
					}
				}
				else
				{
					temporarilyAttachedCamera.transform.position = Vector3.Lerp(temporarilyAttachedCamera.transform.position, permanentlyAttachedCamera.transform.position, Time.deltaTime * headAttachementLerpFactor);
					temporarilyAttachedCamera.transform.rotation = Quaternion.Lerp(temporarilyAttachedCamera.transform.rotation, permanentlyAttachedCamera.transform.rotation, Time.deltaTime * headRotationLerpFactor);
					headHopper.cameraControl.transform.rotation = Quaternion.Lerp(headHopper.cameraControl.transform.rotation, permanentlyAttachedCamera.transform.rotation, Time.deltaTime * headRotationLerpFactor);
				}
			}
			else if((enableHeadTossing == true) && (fireButtonPressed == true))
			{
				permanentlyAttachedCamera.SetActive(false);
				temporarilyAttachedCamera.gameObject.SetActive(true);
				temporarilyAttachedCamera.transform.parent = null;
				temporarilyAttachedCamera.rigidbody.isKinematic = false;
				temporarilyAttachedCamera.rigidbody.velocity = rigidbody.velocity;
				temporarilyAttachedCamera.rigidbody.AddForce(permanentlyAttachedCamera.transform.forward * throwForce, ForceMode.VelocityChange);
				temporarilyAttachedCamera.Start();
				audio.PlayOneShot(throwHeadSound);
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
				temporarilyAttachedCamera.rigidbody.isKinematic = true;
				isHeadAttached = true;
				isAnimatingHeadAttachement = true;
				headHopper.ReattachHead();
				PauseMenu.HideMessage();
				controller.isControlsEnabled = false;
				audio.PlayOneShot(pickupHeadSound);
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
