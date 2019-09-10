using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fisher : MonoBehaviour
{
	public enum ActivatedState
	{
		NotActivated,
		TemporarilyActivated,
		PermanentlyActivated
	}
	
	private static Fisher msInstance = null;
	
	struct RigidBodyStats
	{
		public float mass;
		public float drag;
		public Transform parent;
		public RigidBodyStats(float newMass, float newDrag, Transform newParent)
		{
			mass = newMass;
			drag = newDrag;
			parent = newParent;
		}
	}
	
	public const string ConnectedTag = "ConnectedPoint";
	public const string UnconnectedTag = "UnconnectedPoint";
	
	public float springStrength = 1f;
	public float stringMass = 0.1f;
	public float stringDrag = 0.1f;
	public LineRenderer stringRenderer;
	public Rigidbody anchor;
	public Collectable bait;
	public int numLineRendererPoints = 26;
	public float followVelocity = 2;
	public float distanceBetweenCollectable = 5;
	public Transform parentOfCollectables;
	public Transform particles;
	public MouseLook[] allCameraControllers = null;
	public AudioSource soundEffectAudio = null;
	
	private List<Collectable> mOtherPoints = new List<Collectable>();
	private List<RigidBodyStats> mLastPointStats = new List<RigidBodyStats>();
	private float mTotalMass = 0f;
	private bool mPaused = false;
	
	public static Fisher Instance
	{
		get
		{
			return msInstance;
		}
	}
	
	public int NumPoints
	{
		get
		{
			return mOtherPoints.Count;
		}
	}
	
	public float MaxObtainableMass
	{
		get
		{
			return mTotalMass;
		}
	}
	
	public bool IsMaxedOut
	{
		get
		{
			return (NumPoints >= numLineRendererPoints);
		}
	}
	
	public bool IsPaused
	{
		get
		{
			return mPaused;
		}
		set
		{
			if(mPaused != value)
			{
				// Update the pause value
				mPaused = value;
				
				// Lock the cursor if we're not paused
				Screen.lockCursor = !mPaused;
				
				// Set the timescale to 0 if paused
				float newTimeScale = 1;
				if(mPaused == true)
				{
					newTimeScale = 0;
				}
				Time.timeScale = newTimeScale;
				
				// Update the camera controllers
				if(allCameraControllers != null)
				{
					foreach(MouseLook cameraController in allCameraControllers)
					{
						if(cameraController != null)
						{
							cameraController.enabled = !mPaused;
						}
					}
				}
			}
		}
	}
	
	public bool AddPoint(Collectable newCollectable)
	{
		bool returnFlag = false;
		if((newCollectable != null) && (mOtherPoints.Count < numLineRendererPoints))
		{
			// Grab the last held object, and create a spring joint
			Rigidbody newPoint = newCollectable.GetComponent<Rigidbody>();
			returnFlag = true;
			if(mOtherPoints.Count > 0)
			{
				SpringJoint joint = mOtherPoints[mOtherPoints.Count - 1].gameObject.AddComponent<SpringJoint>();
				if(joint != null)
				{
					joint.spring = springStrength;
					joint.connectedBody = newPoint;
				}
			}
			

			if(returnFlag == true)
			{
				// Update total mass
				mTotalMass += newPoint.mass;
				
				// Store this point's old stats
				mLastPointStats.Add(new RigidBodyStats(newPoint.mass, newPoint.drag, newPoint.transform.parent));
				
				// Update the point with new stats
				newPoint.drag = stringDrag;
				newPoint.mass = stringMass;
				newPoint.useGravity = false;
				
				// Add the new point into the list of points
				mOtherPoints.Add(newCollectable);
				
				// Udpate its tag
				newPoint.tag = ConnectedTag;
				
				// Move the parents of particles and rigidbody
				newPoint.transform.parent = parentOfCollectables;
				particles.transform.parent = newPoint.transform;
				particles.transform.localPosition = Vector3.zero;
			}
		}
		return returnFlag;
	}
	
	public bool PopPoint(out Collectable freeCollectable)
	{
		bool returnFlag = false;
		freeCollectable = null;
		if((mOtherPoints.Count > 1) && (mLastPointStats.Count > 1))
		{
			returnFlag = true;
			if(returnFlag == true)
			{
				// Grab the last point
				freeCollectable = mOtherPoints[mOtherPoints.Count - 1];
				Rigidbody freePoint = freeCollectable.GetComponent<Rigidbody>();
				
				// Return the point to its original stats
				RigidBodyStats lastStats = mLastPointStats[mLastPointStats.Count - 1];
				freePoint.drag = lastStats.drag;
				freePoint.mass = lastStats.mass;
				freePoint.useGravity = true;
				freePoint.transform.parent = lastStats.parent;
				freePoint.tag = UnconnectedTag;
				
				// Update total mass
				mTotalMass -= freePoint.mass;
				
				// Remove the last body and stats
				mOtherPoints.RemoveAt(mOtherPoints.Count - 1);
				mLastPointStats.RemoveAt(mLastPointStats.Count - 1);
			}
			
			// Grab the last held object, and destroy its spring joint
			if(mOtherPoints.Count > 0)
			{
				particles.transform.parent = mOtherPoints[mOtherPoints.Count - 1].transform;
				particles.transform.localPosition = Vector3.zero;
				SpringJoint joint = mOtherPoints[mOtherPoints.Count - 1].gameObject.GetComponent<SpringJoint>();
				if(joint != null)
				{
					Destroy(joint);
				}
			}
		}
		return returnFlag;
	}
	
	void Awake()
	{
		mTotalMass = 0;
		if((anchor != null) && (bait != null))
		{
			SpringJoint joint = anchor.gameObject.AddComponent<SpringJoint>();
			joint.spring = springStrength;
			joint.connectedBody = bait.GetComponent<Rigidbody>();
			AddPoint(bait);
		}
		msInstance = this;
		Screen.lockCursor = true;
	}
	
	void FixedUpdate()
	{
		// Check if we've fired
		if(Singleton.Get<SceneTransition>().State == SceneTransition.Transition.NotTransitioning)
		{
			if(Input.GetButton("Pause") == true)
			{
				IsPaused = !IsPaused;
			}
			else if((Screen.lockCursor == false) && (IsPaused == false) && (Input.GetMouseButtonDown(0) == true))
			{
				Screen.lockCursor = true;
			}
			else if((mOtherPoints.Count > 1) && (Input.GetButtonDown("Fire1") == true))
			{
				Collectable freePoint = null;
				if(PopPoint(out freePoint) == true)
				{
					freePoint.Detach();
				}
			}
		}
		
		// Update the physics
		Ray ray = new Ray(anchor.transform.position, Camera.main.transform.forward);
		Vector3 direction;
		for(int index = 0; index < mOtherPoints.Count; ++index)
		{
			direction = ray.GetPoint((index + 1) * distanceBetweenCollectable);
			direction = direction - mOtherPoints[index].transform.position;
			if(direction.sqrMagnitude > 1)
			{
				direction.Normalize();
			}
			mOtherPoints[index].GetComponent<Rigidbody>().AddForce((direction * followVelocity), ForceMode.VelocityChange);
		}
	}
	
	// Use this for initialization
	void LateUpdate ()
	{
		Transform lastPoint = anchor.transform;
		stringRenderer.SetPosition(0, lastPoint.position);
		for(int index = 1; index < numLineRendererPoints; ++index)
		{
			// Check if there's any other rigidbodies left
			if((index - 1) < mOtherPoints.Count)
			{
				// If there is, grab it's transform
				lastPoint = mOtherPoints[(index - 1)].transform;
			}
			
			// Update the string renderer
			stringRenderer.SetPosition(index, lastPoint.position);
		}
	}
}
