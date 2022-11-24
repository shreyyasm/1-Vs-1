using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class AttackCaller : NetworkBehaviour
{
    PlayerAttackSystem playerAttackSystem;
    private void Update()
    {
        playerAttackSystem = FindObjectOfType<PlayerAttackSystem>();
    }
    public void Attack(float input)
    {
        if(IsOwner)
        {
            if (input == 1)
            {
                playerAttackSystem.Attack();
            }
            if (input == 0)
            {
                playerAttackSystem.Cancel_Block_N_Attack();
            }
        }
        
    }
    public void Block(float input)
    {
        if(IsOwner)
        {
            if (input == 1)
            {
                playerAttackSystem.Block();
            }
            if (input == 0)
            {
                playerAttackSystem.StopBlocking();
            }
        }
        
    }
}
