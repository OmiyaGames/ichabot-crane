using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{
	public static Goal msInstance = null;
	public Color disabledColor = Color.blue;
	public Renderer changeColor = null;
	public Renderer setVisible = null;
	public Collider setCollidable = null;
	public Transform targetPosition = null;
	public ParticleEmitter[] setEmitting = null;
	
	private Color mOriginalColor;
	private Color mTargetColor;
	private Color mCurrentColor;
	private bool mChangeColor = false;
	private bool mWasEnabled = false;
	private bool mIsEnabled = true;
	
	// Use this for initialization
	void Awake ()
	{
		msInstance = this;
		IsEnabled = true;
		if((changeColor != null) && (changeColor.material != null))
		{
			mOriginalColor = changeColor.material.color;
			mTargetColor = mOriginalColor;
			mCurrentColor = mOriginalColor;
			mChangeColor = true;
		}
	}
	
	public Color TargetColor
	{
		get
		{
			Color target = disabledColor;
			if(mIsEnabled == true)
			{
				target = mOriginalColor;
			}
			return target;
		}
	}
	
	public static bool IsEnabled
	{
		set
		{
			if((msInstance != null) && (msInstance.mIsEnabled != value))
			{
				msInstance.mIsEnabled = value;
				if(msInstance.setVisible != null)
				{
					msInstance.setVisible.enabled = value;
				}
				if(msInstance.setCollidable != null)
				{
					msInstance.setCollidable.enabled = value;
				}
				if(msInstance.setEmitting != null)
				{
					foreach(ParticleEmitter emitter in msInstance.setEmitting)
					{
						if(emitter != null)
						{
							emitter.emit = value;
						}
					}
				}
			}
		}
	}
	
	public static Vector3 Position
	{
		get
		{
			Vector3 returnPosition = Vector3.zero;
			if(msInstance != null)
			{
				returnPosition = msInstance.transform.position;
				if(msInstance.targetPosition != null)
				{
					returnPosition = msInstance.targetPosition.position;
				}
			}
			return returnPosition;
		}
	}
	
	void FixedUpdate()
	{
		// Check if we need to change color
		if(mChangeColor == true)
		{
			// Get it's target color
			mTargetColor = TargetColor;
			
			// Update current color
			mCurrentColor = Color.Lerp(mCurrentColor, mTargetColor, (Time.deltaTime * Collectable.ColorChangeSpeed));
			float rDiff = mCurrentColor.r - mTargetColor.r;
			float bDiff = mCurrentColor.b - mTargetColor.b;
			float gDiff = mCurrentColor.g - mTargetColor.g;
			if(((rDiff * rDiff) + (bDiff * bDiff) + (gDiff * gDiff)) < Collectable.ColorDifferenceSnap)
			{
				mCurrentColor = mTargetColor;
				mChangeColor = false;
			}
			
			// Reflect this on all Renderers'
			if((changeColor != null) && (changeColor.material != null))
			{
				changeColor.material.color = mCurrentColor;
			}
		}
		else if(mWasEnabled != mIsEnabled)
		{
			mChangeColor = true;
		}
		
		// Update last frame's obtainable flag
		mWasEnabled = mIsEnabled;
	}
}
