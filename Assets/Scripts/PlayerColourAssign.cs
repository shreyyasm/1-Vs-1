using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerColourAssign : NetworkBehaviour
{
    public GameObject cube;
    public Material serverMaterial;
    public Material clientMaterial;

    ulong playerIndex;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerIndex = OwnerClientId;
        if(playerIndex == 0)
            cube.GetComponent<Renderer>().material = serverMaterial;

        if(playerIndex == 1)
            cube.GetComponent<Renderer>().material = clientMaterial;
    }
}
