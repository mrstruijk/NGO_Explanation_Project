using Unity.Netcode;
using UnityEngine;


/// <summary>
/// Due to some strange behaviour in Unity you're not allowed to have multiple NetworkBehaviours on the same GameObject.
/// This script should therefore be a separate/child of any other NetworkBehaviour script, and reference the NetworkObject you
/// want to influence.
/// </summary>
public class OwnershipManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject m_networkObject;
    private ulong _networkID;

    
    public override void OnNetworkSpawn()
    {
        _networkID = NetworkManager.Singleton.LocalClientId;
    }


    public bool OwnsObject => m_networkObject.IsOwner;


    public void ChangeOwner()
    {
        if (m_networkObject.IsOwner)
        {
            return;
        }

        MakeMineServerRpc(_networkID);
    }


    [ServerRpc(RequireOwnership = false)]
    private void MakeMineServerRpc(ulong networkID)
    {
        m_networkObject.ChangeOwnership(networkID);
    }
}