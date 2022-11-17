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
    
    private float minPunchDistance = 1.5f;

    [SerializeField]
    private GameObject leftHand;

    [SerializeField]
    private GameObject rightHand;

    bool isAttacking = false;

    Animator anim;
    NetworkAnimator networkAnim;

    [SerializeField] ulong playerIndex;
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
        }
        Attack();
        playerIndex = OwnerClientId;
        SetReferenceForPlayer(leftHand.transform, Vector3.left); 
    }
    
    public void CallAttack()
    {
        CheckPunch(leftHand.transform, Vector3.left);
        CheckPunch(rightHand.transform, Vector3.left);
    }
    private void Attack()
    {
        if (IsOwner)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if(IsOwner)
                {
                    anim.SetBool("StrafeForward", false);
                    networkAnim.SetTrigger("Attack");

                    isAttacking = true;
                    playerMovement.StartAttacking();
                }
                    
            }
            if (Input.GetMouseButton(1))
            {
                if (IsOwner)
                {
                    anim.SetBool("StrafeForward", false);
                    anim.SetBool("Block", true);

                    playerMovement.StartAttacking();
                    playerHealth.StartBlocking();
                }
            }
            if(Input.GetMouseButtonUp(1))
            {
                if(IsOwner)
                {
                    anim.SetBool("Block", false);
                    playerMovement.StopAttacking();
                    playerHealth.StopBlocking();
                }                    
            }
        }
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
                if(IsServer)
                {                  
                        CheckPunchServerSide(0);
                }
                if(playerIndex == 1)
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
