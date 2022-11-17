using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Netcode.Components;

public class PlayerHealth : NetworkBehaviour
{
    
    public static PlayerHealth instance;  
    public NetworkVariable<int> networkHealth = new NetworkVariable<int>(100,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> isBlockingNEt = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    HealthBar healthBar;
    ulong playerIndex;

    Animator anim;
    NetworkAnimator networkAnim;

    //public bool serverBlocking = false;
    //public bool clientBlocking = false;
    bool isblocking = false;
    public override void OnNetworkSpawn()
    {
        healthBar = FindObjectOfType<HealthBar>();
        if (IsServer && IsClient)
        {
            healthBar.SetMaxHealth(networkHealth.Value);
        }
    }
    private void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
        }
        anim = gameObject.GetComponent<Animator>();
        networkAnim = gameObject.GetComponent<NetworkAnimator>();
    }
    private void Update()
    {
        playerIndex = OwnerClientId;        
    }
    public void DamageEnemy(int damage)
    {     
          
        if(IsClient)
        {
            if (!isblocking)
            {
                if(networkHealth.Value  > 0)
                {
                    networkHealth.Value -= damage;
                    networkAnim.SetTrigger("Hit");
                    SetHealthClientRPC(networkHealth.Value);
                }
                if(networkHealth.Value == 0)
                {
                    anim.SetBool("Die", true);
                }
            } 
        }
     
    } 
    [ClientRpc]
    public void SetHealthClientRPC(int health)
    {
        
        if (playerIndex == 0)
        {  
            healthBar.SetPlayerHealth(health);
        }
        if (playerIndex == 1)
        {          
            healthBar.SetEnemyHealth(health);
        }

    }
    public void StartBlocking()
    {
        isblocking = true;
    }
    public void StopBlocking()
    {
        isblocking = false;
    }  
}
