using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class Switch : MonoBehaviour {
	public bool triggerOnBody = true;
	public bool triggerOnHead = true;
	public float moveDownBy = 1;
	public float smoothFactor = 1;
	public Renderer colorAdjustment;
	public AudioClip pressedSound;
	public AudioClip upSound;
	public event System.Action<bool> OnSwitchEnter;
	public event System.Action<bool> OnSwitchExit;

	bool isPressed = false;
	Vector3 originalPosition;
	Vector3 pressedPosition;
	Vector3 velocity;
	Color originalColor;
	static readonly Color pressedColor = Color.black;
	bool isTriggerEnabled = true;

	public bool IsTriggerEnabled
	{
		get
		{
			return isTriggerEnabled;
		}
		set
		{
			isTriggerEnabled = value;
		}
	}

	void Start()
	{
		collider.isTrigger = true;
		originalPosition = transform.position;
		pressedPosition = originalPosition;
		pressedPosition += transform.forward * moveDownBy;
		originalColor = colorAdjustment.material.color;
	}

	void OnTriggerEnter(Collider other)
	{
		if((IsTriggerEnabled == true) && (OnSwitchEnter != null) && (other != null))
		{
			if((other.CompareTag("Player") == true) && (triggerOnBody == true))
			{
				isPressed = true;
				audio.PlayOneShot(pressedSound);
				OnSwitchEnter(false);
			}
			else if((other.CompareTag("Player1") == true) && (triggerOnHead == true))
			{
				isPressed = true;
				audio.PlayOneShot(pressedSound);
				OnSwitchEnter(true);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if((IsTriggerEnabled == true) && (OnSwitchExit != null) && (other != null))
		{
			if((other.CompareTag("Player") == true) && (triggerOnBody == true))
			{
				isPressed = false;
				audio.PlayOneShot(upSound);
				OnSwitchExit(false);
			}
			else if((other.CompareTag("Player1") == true) && (triggerOnHead == true))
			{
				isPressed = false;
				audio.PlayOneShot(upSound);
				OnSwitchExit(true);
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{
		Vector3 targetPosition = originalPosition;
		Color targetColor = originalColor;
		if(isPressed == true)
		{
			targetPosition = pressedPosition;
			targetColor = pressedColor;
		}
		
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, Time.deltaTime * smoothFactor);
		colorAdjustment.material.color = Color.Lerp(colorAdjustment.material.color, targetColor, Time.deltaTime * smoothFactor);
	}
}
