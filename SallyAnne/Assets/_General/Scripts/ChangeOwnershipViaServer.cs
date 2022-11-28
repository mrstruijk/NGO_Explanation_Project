using Unity.Netcode;


public class ChangeOwnershipViaServer : NetworkBehaviour
{
    private ulong _networkID;


    public override void OnNetworkSpawn()
    {
        _networkID = NetworkManager.Singleton.LocalClientId;
    }


    public void ChangeOwnerIfNeeded()
    {
        if (!NetworkObject.IsOwner)
        {
            MakeMineServerRpc(_networkID);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void MakeMineServerRpc(ulong networkID)
    {
        NetworkObject.ChangeOwnership(networkID);
    }
}