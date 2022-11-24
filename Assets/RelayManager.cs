
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    public static RelayManager instance;
    [SerializeField]
    private string environment = "production";

    [SerializeField]
    private int maxNumberOfConnections = 10;

    public bool IsRelayEnabled => Transport != null && Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;

    public UnityTransport Transport => NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public async Task SetupRelay()
    {
        //Logger.Instance.LogInfo($"Relay Server Starting With Max Connections: {maxNumberOfConnections}");

        InitializationOptions options = new InitializationOptions()
            .SetEnvironmentName(environment);

        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Allocation allocation = await Relay.Instance.CreateAllocationAsync(maxNumberOfConnections);

        

        var JoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);

        //Transport.SetRelayServerData(relayHostData.IPv4Address, relayHostData.Port, relayHostData.AllocationIDBytes,
                //relayHostData.Key, relayHostData.ConnectionData);


        Transport.SetRelayServerData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

        Logger.Instance.LogInfo($"Relay Server Generated Join Code: {JoinCode}");

        //return tRAns;
    }

    public async Task JoinRelay(string joinCode)
    {
        //Logger.Instance.LogInfo($"Client Joining Game With Join Code: {joinCode}");

        InitializationOptions options = new InitializationOptions()
            .SetEnvironmentName(environment);

        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

       

        //Transport.SetRelayServerData(relayJoinData.IPv4Address, relayJoinData.Port, relayJoinData.AllocationIDBytes,
            //relayJoinData.Key, relayJoinData.ConnectionData, relayJoinData.HostConnectionData);

        Transport.SetRelayServerData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes,
            allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);



        Logger.Instance.LogInfo($"Client Joined Game With Join Code: {joinCode}");

       // return relayJoinData;
    }
}