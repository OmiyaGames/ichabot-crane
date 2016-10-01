using UnityEngine;
using System.Collections;

public class SetupClient : MonoBehaviour
{
	enum State
	{
		AwaitingInput,
		Connecting,
		Connected
	}

	static SetupClient instance = null;
	public float speed = 1;
	public GameObject prefab;
	State currentState = State.AwaitingInput;
	Rect fullRect;
	Rect tempRect;
	Transform cube;
	string ipAddress = string.Empty;

	void Start()
	{
		if(instance == null)
		{
			instance = this;
			GameObject.DontDestroyOnLoad(gameObject);
		}
		else
		{
			GameObject.Destroy(gameObject);
		}
	}

	void Update()
	{
		if(currentState == State.Connected)
		{
			cube.Translate(Vector3.right * Input.GetAxis("Horizontal") * speed);
		}
	}

	void OnGUI()
	{
		fullRect.width = Screen.width / 2;
		fullRect.height = Screen.height / 2;
		fullRect.center = new Vector2(Screen.width / 2, Screen.height / 2);

		switch(currentState)
		{
		case State.AwaitingInput:
			tempRect = fullRect;
			tempRect.height /= 3;
			GUI.Label(tempRect, "Enter server address");

			tempRect.y += tempRect.height;
			ipAddress = GUI.TextField(tempRect, ipAddress);

			tempRect.y += tempRect.height;
			if(GUI.Button(tempRect, "Connect") == true)
			{
				Network.Connect(ipAddress, SetupServer.Port, SetupServer.Password);
				currentState = State.Connecting;
			}
			break;
		case State.Connecting:
			tempRect = fullRect;
			GUI.Label(tempRect, "Connecting...");
			break;
		case State.Connected:
			tempRect = fullRect;
			tempRect.height /= 3;
			GUI.Label(tempRect, "Connected to:");
			
			tempRect.y += tempRect.height;
			GUI.Label(tempRect, ipAddress);

			tempRect.y += tempRect.height;
			if(GUI.Button(tempRect, "Disconnect") == true)
			{
				Network.Disconnect();
			}
			break;
		}
	}

	void OnConnectedToServer()
	{
		//GameObject clone = (GameObject)Network.Instantiate(prefab, Vector3.zero, Quaternion.identity, 1);
		cube = prefab.transform;
		currentState = State.Connected;
		Network.SetSendingEnabled(0, true);
	}

	void OnFailedToConnect(NetworkConnectionError error)
	{
		currentState = State.AwaitingInput;
	}

	void OnDisconnectedFromServer(NetworkDisconnection reason)
	{
		currentState = State.AwaitingInput;
	}

	void OnApplicationQuit()
	{
		Network.Disconnect();
	}
}