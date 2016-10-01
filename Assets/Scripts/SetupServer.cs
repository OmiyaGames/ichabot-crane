using UnityEngine;
using System.Collections;

public class SetupServer : MonoBehaviour {
	public const int Port = 25000;
	public const string Password = "Testing...";
	public const string GameType = "Dodgeball";
	public const string GameName = "Game";

	Rect fullRect;
	System.Text.StringBuilder labelText = new System.Text.StringBuilder();

	// Use this for initialization
	void StartServer () {
		Network.maxConnections = 4;
		Network.incomingPassword = Password;
		Network.InitializeSecurity();
		Network.InitializeServer(32, Port, !Network.HavePublicAddress());
		MasterServer.RegisterHost(GameType, GameName);
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		labelText.AppendLine("Connected player " + player.ipAddress);
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		labelText.AppendLine("Disconnected player " + player.ipAddress);
	}

	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
			{
				StartServer();
			}
		}
		else
		{
			fullRect.width = Screen.width / 2;
			fullRect.height = Screen.height / 2;
			fullRect.center = new Vector2(Screen.width / 2, Screen.height / 2);
			
			GUI.Label(fullRect, labelText.ToString());
		}
	}
}
