using Unity.Netcode;
using UnityEngine;


/// <summary>
///     This class asks the Server / Host to change the position of an object.
///     To make gameplay respond as fast as possible, it will bypass the server-client loop in case the object is owned by
///     me.
///     Based on Tarodev Unity Online Multiplayer
/// </summary>
public class ServerAuthorityChangePosition : NetworkBehaviour
{
    [Header("Set in the Inspector")]
    [SerializeField] private Transform m_objectToMove;

    [Header("Set in the Inspector or will be found")]
    [Tooltip("Finds this component in case it's left empty")]
    [SerializeField] private OwnershipManager m_ownershipManager;
    [Tooltip("Finds this component in case it's left empty")]
    [SerializeField] private Rigidbody m_rigidbody;

    public Transform ObjectToMove => m_objectToMove;


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


    private void Start()
    {
        ObjectToMovePresent();
    }


    /// <summary>
    ///     Can be called from anything which can provide a Vector3 with the new position of the attached object
    /// </summary>
    /// <param name="newPosition"></param>
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


    /// <summary>
    ///     The 'RequireOwnership = false' attribute is because this is called in case we don't yet own the object at the start
    ///     of the call.
    ///     If this attribute wasn't present, you'd have to issue the request twice, since the first time the request can only
    ///     change the ownership at the Server/Host side.
    ///     The confusing this about this call is that it then issues a [ClientRpc], which will only run if we own the object.
    ///     This is because it takes at least one tick for the ownership to change.
    /// </summary>
    /// <param name="newPosition"></param>
    [ServerRpc(RequireOwnership = false)]
    private void ThisClientMovePositionServerRpc(Vector3 newPosition)
    {
        ThisClientMovePositionClientRpc(newPosition);
    }


    /// <summary>
    ///     The [ClientRpc] can only be called by the Server. By default it will request this of all Clients (including the
    ///     Host, since it has a double role as both Server and Client).
    ///     It then checks to see if we own the object, to then move the object.
    /// </summary>
    /// <param name="newPosition"></param>
    [ClientRpc]
    private void ThisClientMovePositionClientRpc(Vector3 newPosition)
    {
        if (IsOwner)
        {
            LocalMovePosition(newPosition);
        }
    }


    /// <summary>
    ///     This asks the Server / Host to issue a [ClientRpc] to move the position of the object.
    /// </summary>
    /// <param name="newPosition"></param>
    [ServerRpc]
    private void OtherClientMovePositionServerRpc(Vector3 newPosition)
    {
        OtherClientMovePositionClientRpc(newPosition);
    }


    /// <summary>
    ///     On every Client which doesn't own the object, the position is changed.
    ///     The (!IsOwner) is called because the owner of the object has already been given the order to change the position
    ///     locally, which increases gameplay snappiness for whoever owns the object (the command then doesn't need need to be
    ///     propagated from the Owner Client to the Server/Host and then back to the Owner Client).
    /// </summary>
    /// <param name="newPosition"></param>
    [ClientRpc]
    private void OtherClientMovePositionClientRpc(Vector3 newPosition)
    {
        if (!IsOwner)
        {
            LocalMovePosition(newPosition);
        }
    }


    /// <summary>
    ///     This changes the position of the object to a new position locally.
    ///     This is initially already called on the Owner's object from the PutObjectHere method earlier.
    /// </summary>
    /// <param name="newPosition"></param>
    private void LocalMovePosition(Vector3 newPosition)
    {
        if (!ObjectToMovePresent())
        {
            return;
        }

        m_objectToMove.position = newPosition;

        m_rigidbody.velocity = new Vector3(0, 0, 0);
        m_rigidbody.angularVelocity = new Vector3(0, 0, 0);
    }


    /// <summary>
    ///     To reduce fatal errors, this checks to see if a Transform has been linked in the inspector.
    /// </summary>
    /// <returns></returns>
    public bool ObjectToMovePresent()
    {
        if (ObjectToMove == null)
        {
            Debug.LogError("We haven't set an object move. This needs to be set in the inspector!");

            return false;
        }

        return true;
    }
}