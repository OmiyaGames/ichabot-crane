using UnityEngine;
using System.Collections;

public class LoadingBar : MonoBehaviour
{
	public GUITexture loadingBar = null;
	public GUITexture[] allOtherTextures = new GUITexture[] {};
	public GUIText[] allTexts = new GUIText[] {};

	private Rect loadingBarDimensions;

	public IEnumerator Start()
	{
		// Check if we're in a webplayer
		bool isWebplayer = true;
		int nextLevel = 1;
		GameSettings settings = Singleton.Get<GameSettings>();
		if (settings != null)
		{
			isWebplayer = settings.IsWebplayer;
		}
		SceneTransition transition = Singleton.Get<SceneTransition> ();
		if (transition != null)
		{
			nextLevel = transition.NextLevel;
		}
		if ((isWebplayer == true) && (Application.CanStreamedLevelBeLoaded(nextLevel) == false))
		{
			// Grab the width of the loading bar
			Rect loadingBarDimensions = loadingBar.pixelInset;
			float originalWidth = loadingBarDimensions.width;
			float originalProgress = Application.GetStreamProgressForLevel(nextLevel);

			// Update the loading bar
			loadingBarDimensions.width = 0;
			loadingBar.pixelInset = loadingBarDimensions;

			// Get the original progress value
			yield return null;

			// Keep updating the loading bar while level is not loaded
			while(Application.CanStreamedLevelBeLoaded(nextLevel) == false)
			{
				// Update the loading bar
				loadingBarDimensions.width = Application.GetStreamProgressForLevel(nextLevel) - originalProgress;
				loadingBarDimensions.width *= originalWidth;
				loadingBar.pixelInset = loadingBarDimensions;
				yield return null;
			}

			// Update the loading bar
			loadingBarDimensions.width = originalWidth;
			loadingBar.pixelInset = loadingBarDimensions;
			yield return null;
		}

		// Directly load to the next level
		Application.LoadLevelAsync(nextLevel);
	}
}
