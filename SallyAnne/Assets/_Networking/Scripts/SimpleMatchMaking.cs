using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ParrelSync;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;


public class SimpleMatchMaking : MonoBehaviour
{
    [SerializeField] private GameObject m_buttons;
    [SerializeField] private int m_maxPlayers = 2;

    private Lobby _connectedLobby;
    private QueryResponse _lobbies;
    private string _playerId;
    private UnityTransport _transport;
    private const string JoinCodeKey = "j";


    private void Awake()
    {
        _transport = FindObjectOfType<UnityTransport>();
    }


    /// <summary>
    ///     Trigger this method via UI or rightclick on the component to call it. 
    ///     This first checks whether a Lobby already exists via QuickJoinLobby, if it doesn't exist, CreateLobby will
    ///     create a new Lobby. If a Lobby already exists, QuickJoinLobby will try to connect to it. This way, this one method
    ///     will handle all network calls.
    /// </summary>
    [ContextMenu(nameof(CreateOrJoinLobby))]
    public async void CreateOrJoinLobby()
    {
        await Authenticate();

        _connectedLobby = await QuickJoinLobby() ?? await CreateLobby();

        if (_connectedLobby == null)
        {
            Debug.LogError("Can neither create new Host, nor connect to other host as a Client. Something has gone terribly wrong.");

            return;
        }

        m_buttons.SetActive(false);
    }


    private async Task Authenticate()
    {
        var options = new InitializationOptions();

        UseParrelSyncProfileName(options);

        await UnityServices.InitializeAsync(options);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        _playerId = AuthenticationService.Instance.PlayerId;

        Debug.LogFormat("Authenticated: {0}", _playerId);
    }


    /// <summary>
    ///     When using Parrel Sync the Profile needs to be renamed for each connected device, since Authentication requires all
    ///     connected devices to be unique. For the project started with ParrelSync the clone's name found in the Clones
    ///     Manager - Argument section will be used (this is 'client' by default), for the other the name 'Primary' will be
    ///     used. This is arbitrary, as long as each device has it's own name.
    ///     From Tarodev: https://youtu.be/fdkvm21Y0xE?t=528
    /// </summary>
    /// <param name="options"></param>
    private static void UseParrelSyncProfileName(InitializationOptions options)
    {
#if UNITY_EDITOR
        var profileName = ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary";
        options.SetProfile(profileName);
        Debug.LogFormat("Profile name is '{0}'", profileName);
#endif
    }


    private async Task<Lobby> QuickJoinLobby()
    {
        try
        {
            Debug.Log("Attempting to QuickJoin Lobby");

            var lobby = await Lobbies.Instance.QuickJoinLobbyAsync();

            Debug.Log(lobby.Name);

            var a = await RelayService.Instance.JoinAllocationAsync(lobby.Data[JoinCodeKey].Value);

            Debug.Log(a.Region);

            SetTransportAsClient(a);

            Debug.Log("Attempting to start Client");

            NetworkManager.Singleton.StartClient();

            Debug.Log("Started Client");

            return lobby;
        }
        catch (Exception e)
        {
            Debug.LogFormat("No lobbies available via QuickJoin, reason: {0}", e);

            return null;
        }
    }


    private void SetTransportAsClient(JoinAllocation a)
    {
        _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort) a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);

        Debug.Log("SetTransportAsClient");
    }


    /// <summary>
    ///     Keeps lobby alive for longer than default, to allow late players to connect.
    ///     By Tarodev: https://youtu.be/fdkvm21Y0xE?t=641
    /// </summary>
    /// <param name="lobbyId"></param>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    private static IEnumerator HeartBeatLobbyKeepAlive(string lobbyId, float waitTime)
    {
        var delay = new WaitForSecondsRealtime(waitTime);

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            Debug.Log("HeartBeat keep Lobby alive");

            yield return delay;
        }
    }


    private async Task<Lobby> CreateLobby()
    {
        try
        {
            Debug.Log("Creating Lobby");

            var a = await RelayService.Instance.CreateAllocationAsync(m_maxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

            var options = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {
                        JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode)
                    }
                }
            };

            var lobby = await Lobbies.Instance.CreateLobbyAsync("Default Lobby Name", m_maxPlayers, options);

            StartCoroutine(HeartBeatLobbyKeepAlive(lobby.Id, 15));

            _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort) a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

            NetworkManager.Singleton.StartHost();

            Debug.Log("Started Host");

            return lobby;
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Failed to create a lobby: {0}", e);

            return null;
        }
    }


    private void OnDestroy()
    {
        try
        {
            if (_connectedLobby == null)
            {
                return;
            }

            if (_connectedLobby.HostId == _playerId)
            {
                Lobbies.Instance.DeleteLobbyAsync(_connectedLobby.Id);
                Debug.Log("Deleted lobby succesfully");
            }
            else
            {
                Lobbies.Instance.RemovePlayerAsync(_connectedLobby.Id, _playerId);
                Debug.Log("Removed player from lobby");
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Error shutting down lobby: {0}", e);

            throw;
        }
    }
}