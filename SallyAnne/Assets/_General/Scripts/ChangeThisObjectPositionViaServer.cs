using Unity.Netcode;
using UnityEngine;


public class ChangeThisObjectPositionViaServer : NetworkBehaviour
{
    [SerializeField] private OwnershipManager m_ownershipManager;
    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private Transform m_objectToMove;

    private void Awake()
    {
        if (m_ownershipManager == null)
        {
            m_ownershipManager = FindObjectOfType<OwnershipManager>();
        }

        if (m_rigidbody == null)
        {
            m_rigidbody = GetComponentInParent<Rigidbody>();
        }
    }


    public void PutObjectHere(Vector3 newPosition)
    {
        if (m_ownershipManager.OwnsObject)
        {
            OtherClientMovePositionServerRpc(newPosition);
            LocalMovePosition(newPosition);
        }
        else
        {
            m_ownershipManager.ChangeOwner();
            ThisClientMovePositionServerRpc(newPosition);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void ThisClientMovePositionServerRpc(Vector3 newPosition)
    {
        ThisClientMovePositionClientRpc(newPosition);
    }


    [ClientRpc]
    private void ThisClientMovePositionClientRpc(Vector3 newPosition)
    {
        if (IsOwner)
        {
            LocalMovePosition(newPosition);
        }
    }


    [ServerRpc]
    private void OtherClientMovePositionServerRpc(Vector3 newPosition)
    {
        OtherClientMovePositionClientRpc(newPosition);
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
        m_objectToMove.position = newPosition;
        Debug.LogFormat("Moving to {0}", newPosition);

        m_rigidbody.velocity = new Vector3(0, 0, 0);
        m_rigidbody.angularVelocity = new Vector3(0, 0, 0);
    }
}