using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    ///     Trigger via button UI or rightclick on the component
    /// </summary>
    [ContextMenu(nameof(CreateOrJoinLobby))]
    public async void CreateOrJoinLobby()
    {
        await Authenticate();

        _connectedLobby = await QuickJoinLobby() ?? await CreateLobby();

        if (_connectedLobby != null)
        {
            m_buttons.SetActive(false);
        }
    }


    private async Task Authenticate()
    {
        var options = new InitializationOptions();

        await UnityServices.InitializeAsync(options);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        _playerId = AuthenticationService.Instance.PlayerId;

        Debug.LogFormat("Authenticated: {0}", _playerId);
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

            SetTransformAsClient(a);

            Debug.Log("Attempting to start Client");

            NetworkManager.Singleton.StartClient();

            Debug.Log("Started Client");

            return lobby;
        }
        catch (Exception e)
        {
            Debug.Log("No lobbies available via QuickJoin");

            return null;
        }
    }


    private void SetTransformAsClient(JoinAllocation a)
    {
        _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort) a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);

        Debug.Log("SetTransformAsClient");
    }


    private static IEnumerator HeartBeatLobbyKeepAlive(string lobbyId, float waitTime)
    {
        var delay = new WaitForSecondsRealtime(waitTime);

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            Debug.Log("A a a a, staying alive, staying alive");

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
            }
            else
            {
                Lobbies.Instance.RemovePlayerAsync(_connectedLobby.Id, _playerId);
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Error shutting down lobby: {0}", e);

            throw;
        }
    }
}