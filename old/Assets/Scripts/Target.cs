using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Target : MonoBehaviour
{
	private static int msNumTargets = 0;
	private static int msNumTriggeredTargets = 0;
	
	public ParticleEmitter particles = null;
	public Renderer targetMesh = null;
	public LineRenderer connectionLine = null;
	public float lineMoveSpeed = 0.1f;
	public AudioClip triggerSound = null;
	public AudioClip goalEnabledSound = null;
	
	private Fisher.ActivatedState mIsActivated = Fisher.ActivatedState.NotActivated;
	private Vector2 mTempOffset;
	
	public Fisher.ActivatedState IsActivated
	{
		get
		{
			return mIsActivated;
		}
	}
	
	void Awake()
	{
		msNumTargets = 0;
		msNumTriggeredTargets = 0;
	}
	
	void Start()
	{
		++msNumTargets;
		Goal.IsEnabled = false;
		collider.isTrigger = true;
		connectionLine.SetPosition(0, transform.position);
		connectionLine.SetPosition(1, Goal.Position);
	}
	
	void Update()
	{
		mTempOffset = connectionLine.material.mainTextureOffset;
		mTempOffset.x += (Time.deltaTime * lineMoveSpeed);
		while(mTempOffset.x > 1)
		{
			mTempOffset.x -= 1;
		}
		connectionLine.material.mainTextureOffset = mTempOffset;
		connectionLine.SetPosition(0, transform.position);
		connectionLine.SetPosition(1, Goal.Position);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if((mIsActivated == Fisher.ActivatedState.NotActivated) && (other != null) && ((other.CompareTag(Fisher.ConnectedTag) == true) || (other.CompareTag(Fisher.UnconnectedTag) == true)))
		{
			// Activate this target
			mIsActivated = Fisher.ActivatedState.PermanentlyActivated;
			particles.Emit();
			targetMesh.enabled = false;
			connectionLine.enabled = false;
			
			// Check if we need to enable the goal
			++msNumTriggeredTargets;
			if(msNumTriggeredTargets >= msNumTargets)
			{
				Goal.IsEnabled = true;
				if((goalEnabledSound != null) && (Fisher.Instance.soundEffectAudio != null))
				{
					Fisher.Instance.soundEffectAudio.PlayOneShot(goalEnabledSound);
				}
			}
			else if((triggerSound != null) && (Fisher.Instance.soundEffectAudio != null))
			{
				Fisher.Instance.soundEffectAudio.PlayOneShot(triggerSound);
			}
		}
	}
}
