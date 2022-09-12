using Unity.Netcode;
using UnityEngine;


[RequireComponent(typeof(NetworkObject))]
public class NetworkSpawner : NetworkBehaviour
{
    private NetworkObject _networkObject;


    private void Awake()
    {
        _networkObject = GetComponent<NetworkObject>();
    }


    public override void OnNetworkSpawn()
    {
        if (_networkObject.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning("Object is not yet active!");
        }

        if (_networkObject.IsSpawned)
        {
            return;
        }

	    _networkObject.Spawn();
    }	
}