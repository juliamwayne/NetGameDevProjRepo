using Unity.Netcode;
using UnityEngine;


// Adapted from https://docs-multiplayer.unity3d.com/netcode/current/tutorials/helloworld
//
// This adds the serverStartType property which allows you to specify how the project
// should be run when running through the Unity editor.

public class GameManager : NetworkBehaviour {

    public NetworkCommandLine.StartModes serverStartType = NetworkCommandLine.StartModes.CHOOSE;

    private GameObject networkCmdlnObj;

    private Color[] playerColors = new Color[]
    {
        Color.blue,
        Color.green,
        Color.yellow,
        Color.gray,
        Color.cyan
    };
    private int colorIndex = 0;

    private void Start() {
        //Since this class has a Mono/Network Behavior attached to it,
        //it should do one thing, and call other classes to it.
        //Particularly, it should call the middle manager of all other classes,
        //so that [some] classes only depend on the middle manager.

        if (Application.isEditor) {
            networkCmdlnObj = GameObject.Find("NetworkCommandLine");
            var networkCmdln = networkCmdlnObj.GetComponent<NetworkCommandLine>();
            networkCmdln.StartAs(serverStartType);
        }
    }


    void OnGUI() {
        //There should be a seperate class that deals only with the GUI.

        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) {
            StartButtons();
        } else {
            StatusLabels();
        }

        GUILayout.EndArea();
    }


    static void StartButtons() {
        //There should be a seperate class that assigns buttons.
        //And also a seperate class that calls buttons.

        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }


    static void StatusLabels() {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }

    [ServerRpc(RequireOwnership = false)] 
    public void RequestNewPlayerColorServerRpc(ServerRpcParams serverRpcParams = default)
    {
        //ServerRpcParams serverRpcParams = default. This is to get the ClientID.

        //RequireOwnership = false. This is common. The server calls this because it's responsible for it.
        //It is not on "void RequestPositionForMovementServerRpc" because the server isn't responsible for what the player does.
        //Setting it to false means any object can call into this.

        Color newColor = playerColors[colorIndex];
        colorIndex += 1;

        if (!IsServer) return;

        if (colorIndex > playerColors.Length - 1)
        {
            colorIndex = 0;
        }

        var playerObject = NetworkManager.Singleton.ConnectedClients[serverRpcParams.Receive.SenderClientId].PlayerObject;
        Player player = playerObject.GetComponent<Player>();
        player.netPlayerColor.Value = newColor;
    }
}