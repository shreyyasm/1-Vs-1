using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class AttackCaller : NetworkBehaviour
{
    public static AttackCaller Instance;
    PlayerAttackSystem server;
    PlayerAttackSystem client;
    [SerializeField] GameObject LoggerScreen;
    [SerializeField] GameObject controls;
    [SerializeField] GameObject attacks;

    bool serverJoined = false;
    bool clientJoined = false;
    bool playerDied = false;
    PlayerMovement playerMovement;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    public override void OnNetworkSpawn()
    {
        if(IsClient && !IsServer)
        FindObjectOfType<PlayerMovement>().ClientHasSpawned();

    }
    private void Update()
    {
        
        if (serverJoined)
        {
            server = GameObject.FindGameObjectWithTag("Server").GetComponent<PlayerAttackSystem>();
        }
            
        
        if(clientJoined)
        {
            client = GameObject.FindGameObjectWithTag("Client").GetComponent<PlayerAttackSystem>();
            LoggerScreen.SetActive(false);
        }
    
        if(playerDied)
        {
            controls.SetActive(false);
            attacks.SetActive(false);
        }
    }
    public void Attack(float input)
    {
        if(server)
        {
            if (input == 1)
            {
                server.Attack();
            }
            if (input == 0)
            {
               server.Cancel_Block_N_Attack();
            }
        }
        if (client)
        {
            if (input == 1)
            {
                client.Attack();
            }
            if (input == 0)
            {
                client.Cancel_Block_N_Attack();
            }
        }

    }
    public void Block(float input)
    {
        if(server)
        {
            if (input == 1)
            {
                server.Block();
            }
            if (input == 0)
            {
                server.StopBlocking();
            }
        }
        if (client)
        {
            if (input == 1)
            {
                client.Block();
            }
            if (input == 0)
            {
                client.StopBlocking();
            }
        }

    }
    public void ServerCheck()
    {
        serverJoined = true;
    }
    public void ClientCheck()
    {
        clientJoined = true;
    }
    [ServerRpc(RequireOwnership = false)]
    public void CheckPlayerDiedServerRPC()
    {
        playerDied = true;
    }
    [ClientRpc]
    public void CheckPlayerDiedClientRPC()
    {
        playerDied = true;
    }
}
