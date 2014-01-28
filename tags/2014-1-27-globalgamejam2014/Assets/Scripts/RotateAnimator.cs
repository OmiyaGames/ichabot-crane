using UnityEngine;
using System.Collections;

public class RotateAnimator : MonoBehaviour
{
	public Vector3 direction = Vector3.forward;
	public float speed = 30f;
	
	private Rigidbody mBody = null;
	
	void Start()
	{
		mBody = GetComponent<Rigidbody>();
		direction.Normalize();
		direction *= speed;
	}
	
	void FixedUpdate ()
	{
		if(mBody == null)
		{
			transform.Rotate(direction * Time.deltaTime);
		}
		else
		{
			Vector3 eular = transform.rotation.eulerAngles;
			eular += (direction * Time.deltaTime);
			mBody.MoveRotation(Quaternion.Euler(eular));
		}
	}
}
