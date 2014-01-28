using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ThrowHead))]
public class GoalDetector : MonoBehaviour
{
	ThrowHead headStatus;

	void Start()
	{
		headStatus = GetComponent<ThrowHead>();
	}

	void OnTriggerEnter(Collider other)
	{
		SceneTransition transition = Singleton.Get<SceneTransition>();
		if((enabled == true) && (other != null) && (headStatus.IsHeadAttached == true) && (transition.State == SceneTransition.Transition.NotTransitioning))
		{
			if(other.CompareTag("Goal") == true)
			{
				Renderer goalRenderer = other.GetComponent<Renderer>();
				if((goalRenderer != null) && (goalRenderer.enabled == true))
				{
					// Figure out which level to switch to next
					int nextLevel = (Application.loadedLevel + 1);
					if(nextLevel > (GameSettings.NumLevels + 1))
					{
						nextLevel = GameSettings.MenuLevel;
					}
					
					// Check if this level is unlocked
					GameSettings settings = Singleton.Get<GameSettings>();
					if(nextLevel > settings.NumLevelsUnlocked)
					{
						// If not, unlock it now!
						settings.NumLevelsUnlocked = nextLevel;
						settings.SaveSettings();
					}
					
					// Disable this script
					enabled = false;
					
					// Transition to the next level
					transition.LoadLevel(nextLevel);
				}
			}
		}
	}
}
