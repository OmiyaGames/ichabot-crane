using UnityEngine;

public class SimpleMouseRotator : MonoBehaviour {
	enum Platform
	{
		Mouse,
		Xbox360,
		PS3
	}
	// A mouselook behaviour with constraints which operate relative to
	// this gameobject's initial rotation.
	
	// Only rotates around local X and Y.
	
	// Works in local coordinates, so if this object is parented
	// to another moving gameobject, its local constraints will
	// operate correctly
	// (Think: looking out the side window of a car, or a gun turret
	// on a moving spaceship with a limited angular range)
	
	// to have no constraints on an axis, set the rotationRange to 360 or greater.

	public Vector2 rotationRange = new Vector3(70,70); 
	public float rotationSpeed = 5;
	public float xboxRotationSpeed = 2.5f;
	public float ps3RotationSpeed = 2.5f;
	public float dampingTime = 0.2f;
	public bool autoZeroVerticalOnMobile = true;
	public bool autoZeroHorizontalOnMobile = false;
	public bool relative = true;
	Vector3 targetAngles;
	Vector3 followAngles;
	Vector3 followVelocity;
	Quaternion originalRotation;

	
	// Use this for initialization
	public void Start () {
		//originalRotation = Quaternion.identity;
		//targetAngles = Vector3.zero;
		//followAngles = Vector3.zero;
		originalRotation = transform.localRotation;
		targetAngles = originalRotation.eulerAngles;
		targetAngles.x = 0;
		originalRotation = Quaternion.Euler(targetAngles);
		targetAngles = Vector3.zero;
		followAngles = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		if(enabled == true)
		{
			// we make initial calculations from the original local rotation
			transform.localRotation = originalRotation;

			// read input from mouse or mobile controls
			float inputH = 0;
			float inputV = 0;
			if (relative)
			{
				Platform platform = Platform.Mouse;
				inputH = CrossPlatformInput.GetAxis("Mouse X");
				if(inputH.Equals(0) == true)
				{
					inputH = Input.GetAxis("Xbox360ControllerRightX");
					platform = Platform.Xbox360;
				}
				if(inputH.Equals(0) == true)
				{
					inputH = Input.GetAxis("PS3ControllerRightX");
					platform = Platform.PS3;
				}

				inputV = CrossPlatformInput.GetAxis("Mouse Y");
				if(inputV.Equals(0) == true)
				{
					inputV = Input.GetAxis("Xbox360ControllerRightY");
					inputV *= -1;
				}
				if(inputV.Equals(0) == true)
				{
					inputV = Input.GetAxis("PS3ControllerRightY");
				}

				// wrap values to avoid springing quickly the wrong way from positive to negative
				if (targetAngles.y > 180) { targetAngles.y -= 360; followAngles.y -= 360; }
				if (targetAngles.x > 180) { targetAngles.x -= 360; followAngles.x-= 360; }
				if (targetAngles.y < -180) { targetAngles.y += 360; followAngles.y += 360; }
				if (targetAngles.x < -180) { targetAngles.x += 360; followAngles.x += 360; }

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
				switch(platform)
				{
					case Platform.Mouse:
					{
						targetAngles.y += inputH * rotationSpeed;
						targetAngles.x += inputV * rotationSpeed;
						break;
					}
					case Platform.Xbox360:
					{
						targetAngles.y += inputH * xboxRotationSpeed;
						targetAngles.x += inputV * xboxRotationSpeed;
						break;
					}
					case Platform.PS3:
					{
						targetAngles.y += inputH * ps3RotationSpeed;
						targetAngles.x += inputV * ps3RotationSpeed;
						break;
					}
				}
				#endif

				// clamp values to allowed range
				targetAngles.y = Mathf.Clamp ( targetAngles.y, -rotationRange.y * 0.5f, rotationRange.y * 0.5f );
				targetAngles.x = Mathf.Clamp ( targetAngles.x, -rotationRange.x * 0.5f, rotationRange.x * 0.5f );

			} else {

				inputH = Input.mousePosition.x;
				inputV = Input.mousePosition.y;

				// set values to allowed range
				targetAngles.y = Mathf.Lerp ( -rotationRange.y * 0.5f, rotationRange.y * 0.5f, inputH/Screen.width );
				targetAngles.x = Mathf.Lerp ( -rotationRange.x * 0.5f, rotationRange.x * 0.5f, inputV/Screen.height );
			}

			// smoothly interpolate current values to target angles
			followAngles = Vector3.SmoothDamp( followAngles, targetAngles, ref followVelocity, dampingTime );

			// update the actual gameobject's rotation
			transform.localRotation = originalRotation * Quaternion.Euler( -followAngles.x, followAngles.y, 0 );
			Debug.Log(gameObject.name + " Rotation");
		}
	}
}
