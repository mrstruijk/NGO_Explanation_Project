using Unity.Netcode;
using UnityEngine;


public class PutMarble : MonoBehaviour
{
    public GameObject Marble;

    private ulong _localClientID;
    private NetworkObject _networkObject;
    private Rigidbody _rigidbody;


    private void Awake()
    {
        _rigidbody = Marble.GetComponent<Rigidbody>();
        _networkObject = Marble.GetComponent<NetworkObject>();
    }


    [ServerRpc(RequireOwnership = false)]
    public void PutMarbleHereServerRpc()
    {
        if (!IsConnected())
        {
            return;
        }

        if (!OwnsObject())
        {
            MakeOwnerServerRpc();
        }

        MovePosition();
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
        return _networkObject.OwnerClientId == NetworkManager.Singleton.LocalClientId;
    }

    [ServerRpc(RequireOwnership = false)]
    private void MakeOwnerServerRpc()
    {
        _networkObject.ChangeOwnership(_localClientID);
        Debug.LogFormat("Changed owner to {0}", _localClientID);
    }


    //TODO: Make sure this happens immediately on all clients.
    private void MovePosition()
    {
        Marble.transform.position = transform.position;
        _rigidbody.velocity = new Vector3(0, 0, 0);
        _rigidbody.angularVelocity = new Vector3(0, 0, 0);
    }
}