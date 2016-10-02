using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Collectable : MonoBehaviour
{
	public const float ColorChangeSpeed = 5f;
	public const float ColorDifferenceSnap = 0.01f;
	public const float DisabledColorGamma = 40f / 256f;
	public static readonly Color DisabledColor = new Color(DisabledColorGamma, DisabledColorGamma, DisabledColorGamma);
	public const float AttachBuffer = 0.2f;
	
	private static float msLastCollectableAttached = -1f;
	
	public float fireVelocity = 50f;
	public float massToSizeRatio = 5f;
	public ParticleEmitter attachParticles;
	public ParticleEmitter fireParticles;
	public AudioSource soundSource;
	public AudioClip attachClip;
	public AudioClip fireClip;
	public Renderer[] allRenderers;
	public bool alwaysEnabled = false;
	
	private Color mTargetColor = DisabledColor;
	private Color mCurrentColor = DisabledColor;
	private Color mOriginalColor = DisabledColor;
	private bool mChangeColor = false;
	private bool mWasObtainable = false;
	private bool mIsObtainable = false;
	
	public bool IsObtainable
	{
		get
		{
			return (mIsObtainable || alwaysEnabled);
		}
	}
	
	public void Detach()
	{
		// Fire
		GetComponent<Rigidbody>().AddForce((Camera.main.transform.forward * fireVelocity), ForceMode.VelocityChange);
		
		// Emit particles
		if(fireParticles != null)
		{
			fireParticles.worldVelocity = Camera.main.transform.forward * -10;
			fireParticles.Emit();
		}
		
		// Play sound
		if((soundSource != null) && (fireClip != null))
		{
			soundSource.PlayOneShot(fireClip);
		}
	}
	
	public Color TargetColor
	{
		get
		{
			Color target = DisabledColor;
			if(IsObtainable == true)
			{
				target = mOriginalColor;
			}
			return target;
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		msLastCollectableAttached = -1f;
		
		// Calculate the mass
		GetComponent<Rigidbody>().mass = transform.lossyScale.magnitude * massToSizeRatio;
		
		// Update whether this object is attainable
		if(Fisher.Instance != null)
		{
			mIsObtainable = (GetComponent<Rigidbody>().mass < Fisher.Instance.MaxObtainableMass);
			mWasObtainable = mIsObtainable;
		}
		
		// Obtain all Renderers
		if((allRenderers == null) || (allRenderers.Length <= 0))
		{
			allRenderers = GetComponentsInChildren<Renderer>();
		}
		
		// Check if there are any renderers
		if((allRenderers != null) && (allRenderers.Length > 0))
		{
			// Get this object's color and texture
			RandomizeSetup randomizerInstance = Singleton.Get<RandomizeSetup>();
			mOriginalColor = randomizerInstance.RandomColor;
			Texture randomTexture = randomizerInstance.RandomTexture;
			
			// Get it's target color
			mTargetColor = TargetColor;
			mCurrentColor = mTargetColor;
			
			// Setup all the renderers
			for(int i = 0; i < allRenderers.Length; ++i)
			{
				if(allRenderers[i] != null)
				{
					foreach(Material mat in allRenderers[i].materials)
					{
						if(mat != null)
						{
							mat.color = mTargetColor;
							mat.mainTexture = randomTexture;
						}
					}
				}
			}
		}
	}
	
	void FixedUpdate()
	{
		// Update whether this object is attainable
		if(Fisher.Instance != null)
		{
			// This is obtainable if the mass is less than the fisher's mass
			mIsObtainable = (GetComponent<Rigidbody>().mass < Fisher.Instance.MaxObtainableMass);
			
			// Check if the fisher is maxed out
			if((Fisher.Instance.IsMaxedOut == true) && (CompareTag(Fisher.UnconnectedTag) == true))
			{
				// If so, it's not obtainable
				mIsObtainable = false;
			}
		}
		
		// Check if we need to change color
		if(mChangeColor == true)
		{
			// Get it's target color
			mTargetColor = TargetColor;
			
			// Update current color
			mCurrentColor = Color.Lerp(mCurrentColor, mTargetColor, (Time.deltaTime * ColorChangeSpeed));
			float rDiff = mCurrentColor.r - mTargetColor.r;
			float bDiff = mCurrentColor.b - mTargetColor.b;
			float gDiff = mCurrentColor.g - mTargetColor.g;
			if(((rDiff * rDiff) + (bDiff * bDiff) + (gDiff * gDiff)) < ColorDifferenceSnap)
			{
				mCurrentColor = mTargetColor;
				mChangeColor = false;
			}
			
			// Reflect this on all Renderers'
			if((allRenderers != null) && (allRenderers.Length > 0))
			{
				for(int i = 0; i < allRenderers.Length; ++i)
				{
					if(allRenderers[i] != null)
					{
						foreach(Material mat in allRenderers[i].materials)
						{
							if(mat != null)
							{
								mat.color = mCurrentColor;
							}
						}
					}
				}
			}
		}
		else if(mWasObtainable != mIsObtainable)
		{
			mChangeColor = true;
		}
		
		// Update last frame's obtainable flag
		mWasObtainable = mIsObtainable;
	}

	void OnCollisionEnter(Collision info)
	{
		if((CompareTag(Fisher.ConnectedTag) == false) &&
			(info.collider != null) &&
			(info.collider.CompareTag(Fisher.ConnectedTag) == true) &&
			(IsObtainable == true) &&
			((Time.time - msLastCollectableAttached) > AttachBuffer))
		{
			// Attach this object to fishing line
			Fisher.Instance.AddPoint(this);
			
			// Emit particles
			if(attachParticles != null)
			{
				attachParticles.Emit();
			}
			
			// Play sound
			if((soundSource != null) && (attachClip != null))
			{
				soundSource.PlayOneShot(attachClip);
			}
			
			msLastCollectableAttached = Time.time;
		}
	}
}
