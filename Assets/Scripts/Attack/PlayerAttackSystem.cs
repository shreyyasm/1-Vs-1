using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerAttackSystem : NetworkBehaviour
{
    public static PlayerAttackSystem instance;

    [SerializeField] int damage = 10;

    NetworkObject playerHit;
    NetworkObject playerHitRef;
    PlayerMovement playerMovement;
    PlayerHealth playerHealth;
    private NetworkVariable<AttackSystem> messageString = new NetworkVariable<AttackSystem>(new AttackSystem {  },NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    
    private float minPunchDistance = 1.7f;

    [SerializeField]
    private GameObject leftHand;

    [SerializeField]
    private GameObject rightHand;

    bool isAttacking = false;

    Animator anim;
    NetworkAnimator networkAnim;

    [SerializeField] ulong playerIndex;
    [SerializeField] CharacterController controller;
    
    struct AttackSystem : INetworkSerializable
    {
        public FixedString128Bytes message;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref message);
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        playerHealth = GetComponent<PlayerHealth>();
        playerMovement = GetComponent<PlayerMovement>();       
        anim = gameObject.GetComponent<Animator>();
        networkAnim = gameObject.GetComponent<NetworkAnimator>();
        playerHit = GetComponent<NetworkObject>();
        playerHitRef = GetComponent<NetworkObject>();
    }
    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f)
        {
            isAttacking = false;
            playerMovement.StopAttacking();
        }

        if (isAttacking)
            controller.Move(Vector3.zero);

        playerIndex = OwnerClientId;
        SetReferenceForPlayer(leftHand.transform, Vector3.left); 
    }
    
    public void CallAttack()
    {
        CheckPunch(leftHand.transform, Vector3.left);
        CheckPunch(rightHand.transform, Vector3.left);
    }
    public void Attack()
    {
       
        if(IsClient)
            SoundManager.Manager.PlaySFX(SoundManager.Punch);
        if (IsOwner)
        {          
            anim.SetBool("StrafeForward", false);
            networkAnim.SetTrigger("Attack");

            isAttacking = true;
            playerMovement.StartAttacking();         
        }
        
    }
    public void Block()
    {
        if(IsOwner)
        {          
             anim.SetBool("StrafeForward", false);
             anim.SetBool("Block", true);
             isAttacking = true;

            playerMovement.StartAttacking();

            if(IsClient)
                playerHealth.StartBlockingServerRPC();  
        }
        
    }
    public void Cancel_Block_N_Attack()
    {
        if(IsOwner)
        {
            anim.SetBool("Block", false);            
            playerHealth.StopBlockingServerRPC();
        }
    }
    public void StopBlocking()
    {
        anim.SetBool("Block", false);
        playerHealth.StopBlockingServerRPC();
        playerMovement.StopAttacking();
    }
    
    public void SetReferenceForPlayer(Transform hand, Vector3 aimDirection)
    {
        RaycastHit hit;

        int layerMask = LayerMask.GetMask("Player");

        if (Physics.Raycast(hand.position, hand.transform.TransformDirection(aimDirection), out hit, minPunchDistance, layerMask))
        {
            Debug.DrawRay(hand.position, hand.transform.TransformDirection(aimDirection) * minPunchDistance, Color.yellow);

            playerHit = hit.transform.GetComponent<NetworkObject>();
            playerHitRef = playerHit;
        }
        else
        {
            Debug.DrawRay(hand.position, hand.transform.TransformDirection(aimDirection) * minPunchDistance, Color.red);
        }
    }
   
    public void CheckPunch(Transform hand, Vector3 aimDirection)
    {
        RaycastHit hit;

        int layerMask = LayerMask.GetMask("Player");

        if (Physics.Raycast(hand.position, hand.transform.TransformDirection(aimDirection), out hit, minPunchDistance, layerMask))
        {
            Debug.DrawRay(hand.position, hand.transform.TransformDirection(aimDirection) * minPunchDistance, Color.yellow);

            playerHit = hit.transform.GetComponent<NetworkObject>();
            playerHitRef = playerHit;
            if (playerHit != null && isAttacking)
            {
                
                if (IsServer)
                {                  
                        CheckPunchServerSide(0);
                }
                if(playerIndex >= 1)
                {
                        CheckPunchServerRPC();                 
                }
            }
        }
        else
        {
            Debug.DrawRay(hand.position, hand.transform.TransformDirection(aimDirection) * minPunchDistance, Color.red);
        }
    }
    [ServerRpc (RequireOwnership = false)]
    public void CheckPunchServerRPC()
    {
        Debug.Log("Hit by player : " + OwnerClientId);       
        playerHitRef.GetComponent<PlayerHealth>().DamageEnemy(damage);       
    }
    [ClientRpc]
    public void CheckPunchClientRPC(ClientRpcParams clientRpcParams)
    {
        Debug.Log("Hit by player : " + OwnerClientId);
        playerHitRef.GetComponent<PlayerHealth>().DamageEnemy(damage);
    }
    private void CheckPunchServerSide(ulong clientId)
    {
        // If is not the Server/Host then we should early return here!
        if (!IsServer) return;


        // NOTE! In case you know a list of ClientId's ahead of time, that does not need change,
        // Then please consider caching this (as a member variable), to avoid Allocating Memory every time you run this function
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        // Let's imagine that you need to compute a Random integer and want to send that to a client        
        CheckPunchClientRPC(clientRpcParams);
    }
}
