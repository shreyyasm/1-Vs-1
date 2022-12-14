using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;



public class HealthBar : NetworkBehaviour
{
	public static HealthBar instance;

	//public NetworkVariable<Vector2> playerbarPosition = new NetworkVariable<Vector2>(new Vector2(836,520), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	//public NetworkVariable<Vector2> enemybarPosition = new NetworkVariable<Vector2>(new Vector2(159, 520), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	[SerializeField] GameObject playerText;
	[SerializeField] GameObject enemyText;


	[SerializeField] GameObject playerPos;
	[SerializeField] GameObject enemyPos;

	public Gradient gradient;

	[SerializeField] Slider playerHealthBar;
	[SerializeField] Image playerFill;

	[SerializeField] Slider enemyHealthBar;
	[SerializeField] Image enemyFill;
	ulong playerIndex;

	[SerializeField] InputField joinCode;
	[SerializeField] GameObject KOGameObject;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			
		}
		playerFill.color = gradient.Evaluate(1f);
		enemyFill.color = gradient.Evaluate(1f);
		playerText.transform.position = enemyPos.transform.position;
		enemyText.transform.position = playerPos.transform.position;
	}
    
    private void Update()
    {
		playerIndex = OwnerClientId;
		if(IsServer)
        {
			HealthBarPosClientRPC();

		}
		if (IsClient)
		{
			
			//SetEnemyHeathBar(1);
			HealthBarPosServerRPC();
		}
		
	
	}
    

	public void SetMaxHealth(int maxHealth)
	{
		if (IsServer)
        {
			playerHealthBar.maxValue = maxHealth;
			enemyHealthBar.maxValue = maxHealth;
			
		}
		
	}
	
	public void SetPlayerHealth(int health)
	{
		playerHealthBar.value = health;
		playerFill.color = gradient.Evaluate(playerHealthBar.normalizedValue);
		
		
	}
	
	public void SetEnemyHealth(int health)
	{

		enemyHealthBar.value = health;
		enemyFill.color = gradient.Evaluate(enemyHealthBar.normalizedValue);
		
	}
	[ClientRpc]
	public void HealthBarPosClientRPC()
    {

		playerHealthBar.transform.position = playerPos.transform.position;
		enemyHealthBar.transform.position = enemyPos.transform.position;
	}
	[ServerRpc(RequireOwnership = false)]
	public void HealthBarPosServerRPC()
	{
		
		playerHealthBar.transform.position = enemyPos.transform.position;
		enemyHealthBar.transform.position = playerPos.transform.position;

	}
	private void SetEnemyHeathBar(ulong clientId)
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
		//HealthBarPosClientRPC(clientRpcParams);
	}
	
	public void Condition()
    {
		if(IsServer)
		SetEnemyHeathBar(1);
	}

	[ServerRpc(RequireOwnership = false)]
	public void KOTextServerRPC()
    {
		KOGameObject.SetActive(true);
		SoundManager.Manager.PlaySFX(SoundManager.KO);
	}
	[ClientRpc]
	public void KOTextClientRPC()
	{
		KOGameObject.SetActive(true);
		SoundManager.Manager.PlaySFX(SoundManager.KO);
	}

}