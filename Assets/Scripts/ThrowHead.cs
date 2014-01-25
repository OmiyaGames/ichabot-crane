using UnityEngine;
using System.Collections;

public class ThrowHead : MonoBehaviour
{
	public GameObject permanentlyAttachedCamera;
	public Rigidbody temporarilyAttachedCamera;
	public float throwForce = 1000;
	bool isHeadAttached = true;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		if((isHeadAttached == true) && (Input.GetAxis("Fire1").CompareTo(0) != 0))
		{
			permanentlyAttachedCamera.SetActive(false);
			temporarilyAttachedCamera.gameObject.SetActive(true);
			temporarilyAttachedCamera.transform.parent = null;
			temporarilyAttachedCamera.AddForce(permanentlyAttachedCamera.transform.forward * throwForce, ForceMode.VelocityChange);
		}
	}
}
