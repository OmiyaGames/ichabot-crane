using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUITexture))]
[RequireComponent(typeof(AudioSource))]
public class SceneTransition : ISingletonScript
{
	public enum Transition
	{
		NotTransitioning,
		FadingOut,
		FadingIn,
		CompletelyFaded
	}
	
	public float fadeInDuration = 0.6f;
	public float fadeInSpeed = 1f;
	public float fadeOutDuration = 1f;
	public float fadeOutSpeed = 5f;
	
	private int mNextLevel = 1;
	private Transition mTransitionState = Transition.NotTransitioning;
	private float mTargetAlpha = 0;
	private float mCurrentAlpha = 0;
	private Color mTargetColor;
	
	public Transition State
	{
		get
		{
			return mTransitionState;
		}
	}

	public int NextLevel
	{
		get
		{
			return mNextLevel;
		}
	}

	public override void SingletonStart()
	{
		mTargetColor = GetComponent<GUITexture>().color;
		mTargetAlpha = 0;
		mCurrentAlpha = 0;
		mTargetColor.a = mTargetAlpha;
		GetComponent<GUITexture>().color = mTargetColor;
		GetComponent<GUITexture>().enabled = false;
	}
	
	public override void SceneStart()
	{
		if(Application.loadedLevel == mNextLevel)
		{
			// Loaded the correct scene, display fade-out transition
			StartCoroutine(FadeOut());
		}
		else if(Application.loadedLevel == 0)
		{
			mTransitionState = Transition.NotTransitioning; 
		}
	}
	
	public void LoadLevel(int levelIndex)
	{
		if((State == Transition.NotTransitioning) && (levelIndex > 0) && (levelIndex <= (GameSettings.NumLevels + 1)))
		{
			// Play sound
			GetComponent<AudioSource>().Play();

			// Set the next level
			mNextLevel = levelIndex;
			
			// Start fading in
			StartCoroutine(FadeIn());
		}
	}
	
	void FixedUpdate()
	{
		// Do the transitioning here
		switch(State)
		{
			case Transition.FadingIn:
			{
				if(GetComponent<GUITexture>().enabled == false)
				{
					mTargetColor = GetComponent<GUITexture>().color;
					mTargetAlpha = 1;
					mCurrentAlpha = 0;
					mTargetColor.a = mTargetAlpha;
					GetComponent<GUITexture>().color = mTargetColor;
					GetComponent<GUITexture>().enabled = true;
				}
				else
				{
					mCurrentAlpha = Mathf.Lerp(mCurrentAlpha, mTargetAlpha, (Time.deltaTime * fadeInSpeed));
					mTargetColor.a = mCurrentAlpha;
					GetComponent<GUITexture>().color = mTargetColor;
				}
				break;
			}
			case Transition.FadingOut:
			{
				mCurrentAlpha = Mathf.Lerp(mCurrentAlpha, mTargetAlpha, (Time.deltaTime * fadeOutSpeed));
				mTargetColor.a = mCurrentAlpha;
				GetComponent<GUITexture>().color = mTargetColor;
				break;
			}
			case Transition.CompletelyFaded:
			{
				mTargetColor = GetComponent<GUITexture>().color;
				mTargetAlpha = 0;
				mCurrentAlpha = 1;
				mTargetColor.a = mCurrentAlpha;
				GetComponent<GUITexture>().color = mTargetColor;
				GetComponent<GUITexture>().enabled = true;
				break;
			}
			default:
			{
				if(GetComponent<GUITexture>().enabled == true)
				{
					GetComponent<GUITexture>().enabled = false;
				}
				break;
			}
		}
	}
	
	IEnumerator FadeIn()
	{
		mTransitionState = Transition.FadingIn;
		yield return new WaitForSeconds(fadeInDuration);
		mTransitionState = Transition.CompletelyFaded;

		// Check if we're in a webplayer
		GameSettings settings = Singleton.Get<GameSettings>();
		if (settings.IsWebplayer == true)
		{
			// If so, load the loading scene
			Application.LoadLevelAsync(0);
		}
		else
		{
			// If not, directly load to the next level
			Application.LoadLevelAsync(mNextLevel);
		}
	}
	
	IEnumerator FadeOut()
	{
		mTransitionState = Transition.FadingOut;
		yield return new WaitForSeconds(fadeOutDuration);
		mTransitionState = Transition.NotTransitioning;
	}
}
