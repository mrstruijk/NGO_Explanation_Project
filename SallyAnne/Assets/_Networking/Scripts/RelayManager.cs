using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Relay;
using UnityEngine;

/// <summary>
/// From: https://www.youtube.com/playlist?list=PLQMQNmwN3FvyyeI1-bDcBPmZiSaDMbFTi
/// </summary>
public class RelayManager : MonoBehaviour
{
    [SerializeField] private bool m_autoStart = false;

    [SerializeField] private int m_maxConnections = 3;

    private const string RelayEnvironment = "production";

    public static bool IsRelayEnabled => Transport != null && Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;

    private static UnityTransport Transport => NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

    public string JoinCode { get; private set; }


    private async void Start()
    {
        if (!m_autoStart)
        {
            return;
        }

        await SetupRelay();
    }


    public async Task<RelayHostData> SetupRelay()
    {
        Debug.Log($"Relay Server is attempting to start with max connections: {m_maxConnections}");

        var options = new InitializationOptions().SetEnvironmentName(RelayEnvironment);

        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var allocation = await Relay.Instance.CreateAllocationAsync(m_maxConnections);

        var relayHostData = new RelayHostData
        {
            Key = allocation.Key,
            Port = (ushort) allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            IPv4Address = allocation.RelayServer.IpV4,
            ConnectionData = allocation.ConnectionData
        };

        relayHostData.JoinCode = await Relay.Instance.GetJoinCodeAsync(relayHostData.AllocationID);

        Transport.SetRelayServerData(relayHostData.IPv4Address, relayHostData.Port, relayHostData.AllocationIDBytes, relayHostData.Key, relayHostData.ConnectionData);

        JoinCode = relayHostData.JoinCode;


        Debug.Log($"Relay Server has been setup. JOIN CODE: {relayHostData.JoinCode}");

        return relayHostData;
    }


    public async Task<RelayJoinData> JoinRelay(string joinCode)
    {
        Debug.Log($"Client is attempting to join with code: {joinCode}");

        var options = new InitializationOptions().SetEnvironmentName(RelayEnvironment);

        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

        var relayJoinData = new RelayJoinData
        {
            Key = allocation.Key,
            Port = (ushort) allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            HostConnectionData = allocation.HostConnectionData,
            IPv4Address = allocation.RelayServer.IpV4,
            JoinCode = joinCode
        };

        if (string.IsNullOrEmpty(JoinCode))
        {
            JoinCode = joinCode;
        }

        Transport.SetRelayServerData(relayJoinData.IPv4Address, relayJoinData.Port, relayJoinData.AllocationIDBytes, relayJoinData.Key, relayJoinData.ConnectionData, relayJoinData.HostConnectionData);


        Debug.Log("Client Joined");

        return relayJoinData;
    }
}