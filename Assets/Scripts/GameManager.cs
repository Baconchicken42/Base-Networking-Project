using Unity.Netcode;
using UnityEngine;


// Adapted from https://docs-multiplayer.unity3d.com/netcode/current/tutorials/helloworld
//
// This adds the serverStartType property which allows you to specify how the project
// should be run when running through the Unity editor.
public class GameManager : NetworkBehaviour {
    public NetworkCommandLine.StartModes serverStartType = NetworkCommandLine.StartModes.CHOOSE;
    private GameObject networkCmdlnObj;

    //?

    public Color[] playerColors = new Color[] { Color.magenta, Color.yellow, Color.blue, Color.grey, Color.cyan };
    public int colorIndex = 0;


    private void Start() {
        if (Application.isEditor) {
            networkCmdlnObj = GameObject.Find("NetworkCommandLine");
            var networkCmdln = networkCmdlnObj.GetComponent<NetworkCommandLine>();
            networkCmdln.StartAs(serverStartType);
        }

        
    }


    void OnGUI() {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) {
            StartButtons();
        } else {
            StatusLabels();
        }

        GUILayout.EndArea();
    }


    static void StartButtons() {
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

    //for other objects to call this, ownership is optional. (client is not the owner of gamemanager)
    [ServerRpc(RequireOwnership = false)]
    public void RequestNewPlayerColorServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (!IsServer)
            return;

        var po = NetworkManager.Singleton.ConnectedClients[serverRpcParams.Receive.SenderClientId].PlayerObject;
        Player player = po.GetComponent<Player>();

        Color newColor = playerColors[colorIndex];
        colorIndex += 1;
        if (colorIndex > playerColors.Length - 1)
            colorIndex = 0;        

        player.playerColor.Value = newColor;
    }

    
}