using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.EventSystems;
using Unity.Netcode.Components;

public class PlayerMovement : NetworkBehaviour
{
    public static PlayerMovement instance;

    [SerializeField] Vector3 spawnPositionServer;
    [SerializeField] Vector3 spawnPositionClient;
    
    [SerializeField] float speed = 6f;
    [SerializeField] float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public Camera cam;

    [SerializeField] float rotationSpeed;
    Vector3 movement;

    public bool isAttacking = false;
    bool clientSpawned = false;

    Animator anim;
    NetworkAnimator networkAnim;

    public CharacterController controller;
    ulong playerIndex;

    

    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        networkAnim = gameObject.GetComponent<NetworkAnimator>();

        if(instance == null)
        {
            instance = this;
        }
        
        
    }
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            gameObject.AddComponent<CameraFollow>();

        }
        if (IsClient)
        {
            //SetPositionServerRPC();
            transform.position = spawnPositionServer;
        }
        if (IsServer)
        {
            SetClientPosition(0);
        }
    }
    public void ClientConnected()
    {
        ClientHasSpawned();
    }
    private void Update()
    {
        cam = GameObject.FindObjectOfType<Camera>();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f)
        {
            isAttacking = false;
        }
        if (clientSpawned)
        {
            controller.Move(Vector3.zero);
        }
        Move();

        playerIndex = OwnerClientId;
        if (playerIndex == 0)
        {
            AttackCaller.Instance.ServerCheck();
            gameObject.tag = "Server";
            if (!clientSpawned)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }

        }
        if (playerIndex >= 1)
        {
            AttackCaller.Instance.ClientCheck();
            gameObject.tag = "Client";
            
        }
    
    }
    
    private void Move()
    {
        if(IsOwner)
        {
           
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            float h = UltimateJoystick.GetHorizontalAxis("Movement");
            float v = UltimateJoystick.GetVerticalAxis("Movement");
            Vector3 direction = new Vector3(h, 0f, v).normalized;
           
            if(direction.magnitude >= 0.1f && !isAttacking)
            {
                
                ClientHasSpawned();
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

                transform.rotation = Quaternion.Euler(0, angle, 0);
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
                
                anim.SetBool("StrafeForward", true);
       
            }
            
            if (direction == Vector3.zero && !isAttacking)
            {
                if(clientSpawned)
                {
                    controller.Move(Vector3.zero);
                }
                
                anim.SetBool("StrafeForward", false);             
                networkAnim.SetTrigger("Idle");
            }
        }
    }
    public void StartAttacking()
    {
        isAttacking = true;
        
    }
    public void StopAttacking()
    {
        isAttacking = false;
    }  
   
    [ClientRpc]
    public void SetPositionClientRPC(ClientRpcParams clientRpcParams)
    {
        transform.position = spawnPositionClient;
    }
    private void SetClientPosition(ulong clientId)
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
        SetPositionClientRPC(clientRpcParams);
    }
    public void ClientHasSpawned()
    {
        clientSpawned = true;
    }
}

