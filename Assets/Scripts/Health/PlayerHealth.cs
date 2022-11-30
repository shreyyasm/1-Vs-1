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
        healthBar = FindObjectOfType<HealthBar>();

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
                    StartCoroutine(ShowEndMenu());
                   
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
        if (playerIndex >= 1)
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
   IEnumerator ShowEndMenu()
    {
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
