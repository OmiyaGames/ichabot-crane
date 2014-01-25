using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class TriggerMessage : MonoBehaviour
{
	public string message;
	
	private bool mIsTriggered = false;
	
	void Awake()
	{
		collider.isTrigger = true;
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if((mIsTriggered == false) && (other.CompareTag("Player") == true))
		{
			PauseMenu.ShowMessage(message);
			mIsTriggered = true;
		}
	}
}
