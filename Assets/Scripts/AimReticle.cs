using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUITexture))]
public class AimReticle : MonoBehaviour
{
	static GUITexture reticle;
	public static GUITexture Reticle
	{
		get
		{
			return reticle;
		}
	}
	
	// Use this for initialization
	void Awake ()
	{
		reticle = GetComponent<GUITexture>();
	}
	
	// Update is called once per frame
	void OnDestroy ()
	{
		reticle = null;
	}
}
