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
        if (NetworkObject.IsOwner)
        {
            OtherClientMovePositionServerRpc(newPosition);
            LocalMovePosition(newPosition);
        }
        else
        {
            MakeMineServerRpc(_networkID);
            ThisClientMovePositionServerRpc(newPosition);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void MakeMineServerRpc(ulong networkID)
    {
        NetworkObject.ChangeOwnership(networkID);
    }


    [ServerRpc(RequireOwnership = false)]
    private void ThisClientMovePositionServerRpc(Vector3 newPosition)
    {
        ThisClientMovePositionClientRpc(newPosition);
    }


    [ServerRpc]
    private void OtherClientMovePositionServerRpc(Vector3 newPosition)
    {
        OtherClientMovePositionClientRpc(newPosition);
    }


    [ClientRpc]
    private void ThisClientMovePositionClientRpc(Vector3 newPosition)
    {
        if (IsOwner)
        {
            LocalMovePosition(newPosition);
        }
    }


    [ClientRpc]
    private void OtherClientMovePositionClientRpc(Vector3 newPosition)
    {
        if (!IsOwner)
        {
            LocalMovePosition(newPosition);
        }
    }


    private void LocalMovePosition(Vector3 newPosition)
    {
        transform.position = newPosition;
        Debug.LogFormat("Moving to {0}", newPosition);

        _rigidbody.velocity = new Vector3(0, 0, 0);
        _rigidbody.angularVelocity = new Vector3(0, 0, 0);
    }
}