using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class HopHead : MonoBehaviour {
	public float upForce = 10;
	public float forwardForce = 10;
	private const float jumpRayLength = 1.5f; 
	public bool grounded { get; private set; }
	private SphereCollider headCollider;

	// Use this for initialization
	void Start () {
		// Set up a reference to the capsule collider.
		headCollider = collider as SphereCollider;
		grounded = true;
	}

	public void Jump()
	{
		// add jump power
		if (grounded == true)
		{
			rigidbody.AddForce(transform.up * upForce + transform.forward * forwardForce, ForceMode.VelocityChange);
			grounded = false;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		// Ground Check:
		
		// Create a ray that points down from the centre of the character.
		Ray ray = new Ray(transform.position, -transform.up);
		
		// Raycast slightly further than the capsule (as determined by jumpRayLength)
		RaycastHit[] hits = Physics.RaycastAll(ray, headCollider.radius * jumpRayLength );
		
		
		float nearest = Mathf.Infinity;
		if (grounded || rigidbody.velocity.y < 0.1f)
		{
			// Default value if nothing is detected:
			grounded = false;
			
			// Check every collider hit by the ray
			for (int i = 0; i < hits.Length; i++)
			{
				// Check it's not a trigger
				if (!hits[i].collider.isTrigger && hits[i].distance < nearest)
				{
					// The character is grounded, and we store the ground angle (calculated from the normal)
					grounded = true;
					nearest = hits[i].distance;
					//Debug.DrawRay(transform.position, groundAngle * transform.forward, Color.green);
				}
			}
		}

		Debug.DrawRay(ray.origin, ray.direction * headCollider.radius * jumpRayLength, grounded ? Color.green : Color.red );
	}
}
