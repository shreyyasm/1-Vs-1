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

    [SerializeField] GameObject PlayerUI;
    [SerializeField] InputField joinCode;
    // Start is called before the first frame update
    void Start()
    {
        hostButton.onClick.AddListener(async () =>
        {
            if (RelayManager.instance.IsRelayEnabled)
            {
                await RelayManager.instance.SetupRelay();
            }
            NetworkManager.Singleton.StartHost();
            UImanage.SetActive(false);
            //PlayerUI.SetActive(true);

            Logger.Instance.LogInfo("Host Connected");
            
            

                
        });

        serverButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            UImanage.SetActive(false);
            shutDown.SetActive(true);
        });

        clientButton.onClick.AddListener(async () =>
        {
            if (RelayManager.instance.IsRelayEnabled && !string.IsNullOrEmpty(joinCode.text))
                await RelayManager.instance.JoinRelay(joinCode.text);

            NetworkManager.Singleton.StartClient();
            //HealthBar.instance.CheckPlayerJoined();
            UImanage.SetActive(false);
            shutDown.SetActive(true);
            Logger.Instance.LogInfo("Client Connected");
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
