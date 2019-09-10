using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class TriggerMessage : MonoBehaviour
{
	public string message;
	
	private bool mIsTriggered = false;
	
	void Awake()
	{
		GetComponent<Collider>().isTrigger = true;
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if((mIsTriggered == false) && ((other.CompareTag("Player") == true) || (other.CompareTag("Player1") == true)))
		{
			PauseMenu.ShowMessage(message);
			mIsTriggered = true;
		}
	}
}
