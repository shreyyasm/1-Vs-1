using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class UIManager : MonoBehaviour
{
    [SerializeField] Button hostButton;
    [SerializeField] Button serverButton;
    [SerializeField] Button clientButton;

    [SerializeField] GameObject UImanage;
    [SerializeField] GameObject shutDown;
    [SerializeField] Button leaveButton;
    // Start is called before the first frame update
    void Start()
    {
        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            UImanage.SetActive(false);
            //shutDown.SetActive(true);
            Debug.Log("Host Connected");
            //CameraFollow.instance.GameStart();
        });

        serverButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            UImanage.SetActive(false);
            shutDown.SetActive(true);
        });

        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            //HealthBar.instance.CheckPlayerJoined();
            UImanage.SetActive(false);
            shutDown.SetActive(true);
            Debug.Log("Client Connected");
            HealthBar.instance.Condition();

        });

        leaveButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            UImanage.SetActive(true);
            shutDown.SetActive(false);
        });
    }

   
}
