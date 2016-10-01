using UnityEngine;
using System.Collections;

public class Fan : MonoBehaviour
{
	public float activateSpeed = 2f;
	public float permanentSpeed = 5f;
	public Rigidbody fanBlades = null;
	public bool stayActivated = true;
	
	private Fisher.ActivatedState mIsActivated = Fisher.ActivatedState.NotActivated;
	private Vector3 mSpinTorque;
	
	public float FanBladeSpeed
	{
		get
		{
			float returnSpeed = 0;
			if(fanBlades != null)
			{
				returnSpeed = fanBlades.angularVelocity.magnitude;
			}
			return returnSpeed;
		}
	}
	
	public Fisher.ActivatedState FanBladeActivated
	{
		get
		{
			return mIsActivated;
		}
	}
	
	void FixedUpdate()
	{
		if(fanBlades != null)
		{
			// Check if this fan is permanently activated
			if(mIsActivated == Fisher.ActivatedState.PermanentlyActivated)
			{
				// If it is, keep spinning
				fanBlades.AddRelativeTorque(mSpinTorque);
			}		
			else
			{
				// Check if the blade speed exceeds activation
				mIsActivated = Fisher.ActivatedState.NotActivated;
				if(FanBladeSpeed > activateSpeed)
				{
					// Check if we should permanently activated
					mIsActivated = Fisher.ActivatedState.TemporarilyActivated;
					if(stayActivated == true)
					{
						// Setup the torque speed
						mIsActivated = Fisher.ActivatedState.PermanentlyActivated;
						mSpinTorque = fanBlades.angularVelocity;
						mSpinTorque.Normalize();
						mSpinTorque *= permanentSpeed;
					}
				}
			}
		}
	}
}
