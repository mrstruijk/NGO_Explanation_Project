using Unity.Netcode;
using UnityEngine;


/// <summary>
///     Put this script on a gameobject, and link the NetworkObject which should be managed in the Inspector.
///     This class can then change the Ownership of said NetworkObject to the local client.
/// </summary>
public class OwnershipManager : NetworkBehaviour
{
    [Header("Set in the Inspector or will be found")]
    [SerializeField] private NetworkObject m_networkObject;
    private ulong _thisClientNetworkID;


    /// <summary>
    ///     This is here to more easily find out if the object is owned by the local Client or not.
    /// </summary>
    public bool OwnsObject => FoundNetworkObject() && m_networkObject.IsOwner;


    private void Awake()
    {
        if (m_networkObject == null)
        {
            m_networkObject = GetComponent<NetworkObject>();
        }

        if (m_networkObject == null)
        {
            m_networkObject = GetComponentInParent<NetworkObject>();
        }
    }


    private void Start()
    {
        FoundNetworkObject();
    }


    /// <summary>
    ///     As soon as the Network connects, the LocalClientId will be stored.
    ///     _thisClientNetworkID will be passed onto the Server to request the object's ownership to be changed to this local
    ///     client.
    /// </summary>
    public override void OnNetworkSpawn()
    {
        _thisClientNetworkID = NetworkManager.Singleton.LocalClientId;
    }


    /// <summary>
    ///     Call this via UI, or from a different script.
    /// </summary>
    public void ChangeOwner()
    {
        if (m_networkObject.IsOwner)
        {
            return;
        }

        MakeMineServerRpc(_thisClientNetworkID);
    }


    /// <summary>
    ///     Since we don't yet own the object, the 'RequireOwnership = false' flag needs to be attached.
    ///     The Server/Host will then change the ownership of the object to the local client.
    /// </summary>
    /// <param name="networkID"></param>
    [ServerRpc(RequireOwnership = false)]
    private void MakeMineServerRpc(ulong networkID)
    {
        if (!FoundNetworkObject())
        {
            return;
        }

        m_networkObject.ChangeOwnership(networkID);
    }


    /// <summary>
    ///     To reduce fatal errors, this checks to see if a NetworkObject has been found or linked in the inspector.
    /// </summary>
    /// <returns></returns>
    private bool FoundNetworkObject()
    {
        if (m_networkObject == null)
        {
            Debug.LogError("No NetworkObject was found on this gameobject or it's parent!");

            return false;
        }

        return true;
    }
}