using Unity.Netcode;
using UnityEngine;


public class ChangeThisObjectPositionViaServer : NetworkBehaviour
{
    private ulong _localClientID;
    private Rigidbody _rigidbody;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }


    public override void OnNetworkSpawn()
    {
        _localClientID = NetworkManager.Singleton.LocalClientId;
        Debug.LogWarning(_localClientID);
    }


    public void PutObjectHere(Transform newTransform)
    {
        MovePositionServerRpc(newTransform.position);
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