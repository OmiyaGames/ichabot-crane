using UnityEngine;
using System.Collections;

public class SetupServer : MonoBehaviour {
	public const int Port = 25000;
	public const string Password = "Testing...";

	// Use this for initialization
	void Start () {
		Network.maxConnections = 4;
		Network.incomingPassword = Password;
		Network.InitializeServer(32, Port);
	}
}
