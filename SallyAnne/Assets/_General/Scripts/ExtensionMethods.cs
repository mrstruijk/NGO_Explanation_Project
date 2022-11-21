using Unity.Netcode;
using UnityEngine;


public static class ExtensionMethods
{
    public static bool IsConnected(this NetworkManager networkManager)
    {
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer) // IsHost checks for both IsClient and IsHost
        {
            Debug.Log(networkManager.name + " is connected");
            return true;
        }

        Debug.LogError("We're not connected yet!");

        return false;
    }
}