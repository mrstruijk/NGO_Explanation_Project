using Unity.Netcode;
using UnityEngine;


public class ChangeMarblePosition : NetworkBehaviour
{
    private ulong _localClientID;
    private NetworkObject _networkObject;
    private Rigidbody _rigidbody;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _networkObject = GetComponent<NetworkObject>();
    }


    public override void OnNetworkSpawn()
    {
        _localClientID = NetworkManager.Singleton.LocalClientId;
        Debug.LogWarning(_localClientID);
    }


    public void PutMarbleHere(Transform newTransform)
    {
        if (!IsConnected())
        {
            return;
        }

        if (!OwnsObject())
        {
            Debug.Log("I do not own the Marble");
            MakeOwnerServerRpc(_localClientID);
        }
        else
        {
            Debug.Log("The marble is mine!");
        }

        MovePositionServerRpc(newTransform.position);
    }


    private static bool IsConnected()
    {
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            return true;
        }

        Debug.LogError("We're not connected yet!");

        return false;
    }


    private bool OwnsObject()
    {
        return _networkObject.IsOwner;
    }


    [ServerRpc(RequireOwnership = false)]
    private void MakeOwnerServerRpc(ulong localClientID)
    {
        _networkObject.ChangeOwnership(localClientID);
        Debug.LogFormat("Changed owner to {0}", localClientID);
    }


    //TODO: Make sure this happens immediately on all clients.
    [ServerRpc(RequireOwnership = false)]
    private void MovePositionServerRpc(Vector3 newPosition)
    {
        transform.position = newPosition;
        Debug.LogFormat("Moving to {0}", newPosition);

        _rigidbody.velocity = new Vector3(0, 0, 0);
        _rigidbody.angularVelocity = new Vector3(0, 0, 0);
    }
}