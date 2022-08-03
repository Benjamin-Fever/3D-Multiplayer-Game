using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkButtons : MonoBehaviour
{

    string ip = "127.0.0.1";
    string strPort = "7777";
    private short port;
    
    private void Awake(){
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-launch-as-server")
            {
                NetworkManager.Singleton.StartServer();
            }
        }
    }

    private void OnGUI() {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient)
        {
            Cursor.lockState = CursorLockMode.Confined;
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            ip = GUILayout.TextField(ip, 15);

            strPort = GUILayout.TextField(strPort, 5);
            if (short.TryParse(strPort, out port))
            {
                GetComponent<UnityTransport>().SetConnectionData(ip, (ushort)port);
            }
        }

        GUILayout.EndArea();
    }

    // private void Awake() {
    //     GetComponent<UnityTransport>().SetDebugSimulatorParameters(
    //         packetDelay: 120,
    //         packetJitter: 5,
    //         dropRate: 3);
    // }
}