using UnityEngine;
using System.Collections;

public class TestCubeSync : MonoBehaviour {
	public float speed = 10f;
	
	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncPosition = Vector3.zero;
		if (stream.isWriting)
		{
			syncPosition = transform.position;
			stream.Serialize(ref syncPosition);
		}
		else
		{
			stream.Serialize(ref syncPosition);

			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			
			syncEndPosition = syncPosition;
			syncStartPosition = transform.position;
		}
	}
	
	void Awake()
	{
		lastSynchronizationTime = Time.time;
	}
	
	void Update()
	{
		if (networkView.isMine)
		{
			InputMovement();
		}
		else
		{
			SyncedMovement();
		}
	}
	
	
	private void InputMovement()
	{
		if (Input.GetKey(KeyCode.W))
			transform.Translate(Vector3.forward * speed * Time.deltaTime);
		
		if (Input.GetKey(KeyCode.S))
			transform.Translate(Vector3.back * speed * Time.deltaTime);
		
		if (Input.GetKey(KeyCode.D))
			transform.Translate(Vector3.right * speed * Time.deltaTime);
		
		if (Input.GetKey(KeyCode.A))
			transform.Translate(Vector3.left * speed * Time.deltaTime);
	}
	
	private void SyncedMovement()
	{
		syncTime += Time.deltaTime;
		
		transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
	}
}
