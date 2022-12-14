using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Netcode.Components;
using UnityEngine.SceneManagement;

public class PlayerHealth : NetworkBehaviour
{
    
    public static PlayerHealth instance;  
    public NetworkVariable<int> networkHealth = new NetworkVariable<int>(100,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> isBlockingNEt = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    HealthBar healthBar;
    public ulong playerIndex;

    Animator anim;
    NetworkAnimator networkAnim;

    bool playerDead = false;
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
                if(networkHealth.Value <= 0)
                {
                    
                    playerDead = true;
                    networkAnim.SetTrigger("Die");
                    AttackCaller.Instance.CheckPlayerDiedServerRPC();
                    AttackCaller.Instance.CheckPlayerDiedClientRPC();
                    HealthBar.instance.KOTextServerRPC();     
                    StartCoroutine(ShowEndMenu());
                   
                }
            }
           
        }
     
    } 
    [ClientRpc]
    public void SetHealthClientRPC(int health)
    {

        SoundManager.Manager.PlaySFX(SoundManager.Hurt);
        if (playerIndex == 0)
        {  
            healthBar.SetPlayerHealth(health);
        }
        if (playerIndex >= 1)
        {          
            healthBar.SetEnemyHealth(health);
        }

    }
    [ServerRpc(RequireOwnership = false)]
    public void StartBlockingServerRPC()
    {
        isblocking = true;
    }
    [ServerRpc(RequireOwnership = false)]
    public void StopBlockingServerRPC()
    {
        isblocking = false;
    }  
    IEnumerator ShowEndMenu()
    {
        
        if (IsServer)
            HealthBar.instance.KOTextClientRPC();

        if (IsClient)
            HealthBar.instance.KOTextServerRPC(); 

        yield return new WaitForSeconds(5f);
        if (IsServer)
            EndClientRPC();

        if (IsClient)
            EndServerRPC();


    }
    [ServerRpc(RequireOwnership = false)]
    public void EndServerRPC()
    {
        UIManager.instance.ShowEndMenu();
        Debug.Log("workaaa");
    }
    [ClientRpc]
    public void EndClientRPC()
    {
        UIManager.instance.ShowEndMenu();
        Debug.Log("workaaa");
    }

}
