using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class RollHead : MonoBehaviour {
	// A mouselook behaviour with constraints which operate relative to
	// this gameobject's initial rotation.
	
	// Only rotates around local X and Y.
	
	// Works in local coordinates, so if this object is parented
	// to another moving gameobject, its local constraints will
	// operate correctly
	// (Think: looking out the side window of a car, or a gun turret
	// on a moving spaceship with a limited angular range)
	
	// to have no constraints on an axis, set the rotationRange to 360 or greater.
	
	public float rotationSpeed = 10;
	public float dampingTime = 0.2f;
	public bool autoZeroVerticalOnMobile = true;
	public bool autoZeroHorizontalOnMobile = false;
	Vector3 targetAngles;
	Vector3 followAngles;
	Vector3 followVelocity;

	// Update is called once per frame
	void FixedUpdate () {

		// read input from mouse or mobile controls
		float inputH = CrossPlatformInput.GetAxis("Mouse X");
		float inputV = CrossPlatformInput.GetAxis("Mouse Y");

		#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
		// on mobile, sometimes we want input mapped directly to tilt value,
		// so it springs back automatically when the look input is released.
		if (autoZeroHorizontalOnMobile) {
			targetAngles.y = Mathf.Lerp (-rotationRange.y * 0.5f, rotationRange.y * 0.5f, inputH * .5f + .5f);
		} else {
			targetAngles.y += inputH * rotationSpeed;
		}
		if (autoZeroVerticalOnMobile) {
			targetAngles.x = Mathf.Lerp (-rotationRange.x * 0.5f, rotationRange.x * 0.5f, inputV * .5f + .5f);
		} else {
			targetAngles.x += inputV * rotationSpeed;
		}
		#else
		// with mouse input, we have direct control with no springback required.
		targetAngles.y = inputH * rotationSpeed;
		targetAngles.x = inputV * -rotationSpeed;
		#endif
		
		// smoothly interpolate current values to target angles
		followAngles = Vector3.SmoothDamp( followAngles, targetAngles, ref followVelocity, dampingTime );
		
		// update the actual gameobject's rotation
		rigidbody.AddRelativeTorque(followAngles, ForceMode.Force);
	}
}
