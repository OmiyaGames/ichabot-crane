using UnityEngine;
using System.Collections;

public class UnmovableObject : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		// Set all materials' color and texture
		Texture randomTexture = Singleton.Get<RandomizeSetup>().RandomTexture;
		foreach(Material mat in GetComponent<Renderer>().materials)
		{
			if(mat != null)
			{
				mat.mainTexture = randomTexture;
			}
		}
	}
}
