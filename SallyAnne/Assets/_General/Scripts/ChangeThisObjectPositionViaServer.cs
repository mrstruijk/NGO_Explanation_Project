using Unity.Netcode;
using UnityEngine;


public class ChangeThisObjectPositionViaServer : NetworkBehaviour
{
    private ulong _networkID;
    private Rigidbody _rigidbody;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }


    public override void OnNetworkSpawn()
    {
        _networkID = NetworkManager.Singleton.LocalClientId;
    }


    public void PutObjectHere(Vector3 newPosition)
    {
        if (!NetworkObject.IsOwner)
        {
            MakeMineServerRpc(_networkID, newPosition);
        }

        MovePositionServerRpc(newPosition);
        MovePosition(newPosition);
    }


    [ServerRpc(RequireOwnership = false)]
    private void MakeMineServerRpc(ulong networkID, Vector3 newPosition)
    {
        NetworkObject.ChangeOwnership(networkID);
        MovePositionImmediatelyClientRpc(newPosition);
    }

    
    [ServerRpc]
    private void MovePositionServerRpc(Vector3 newPosition)
    {
        MovePositionClientRpc(newPosition);
    }


    [ClientRpc]
    private void MovePositionClientRpc(Vector3 newPosition)
    {
        if (!IsOwner)
        {
            MovePosition(newPosition);
        }
    }

    [ClientRpc]
    private void MovePositionImmediatelyClientRpc(Vector3 newPosition)
    {
        if (IsOwner)
        {
            MovePosition(newPosition);
        }
    }

    private void MovePosition(Vector3 newPosition)
    {
        transform.position = newPosition;
        Debug.LogFormat("Moving to {0}", newPosition);

        _rigidbody.velocity = new Vector3(0, 0, 0);
        _rigidbody.angularVelocity = new Vector3(0, 0, 0);
    }
}