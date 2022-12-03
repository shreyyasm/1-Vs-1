using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
public class UIManager : NetworkBehaviour
{
    public static UIManager instance;

    [SerializeField] Button hostButton;
    [SerializeField] Button serverButton;
    [SerializeField] Button clientButton;

    [SerializeField] GameObject endMenu;
    [SerializeField] Button RestartButton;
    [SerializeField] Button QuitButton;

    [SerializeField] GameObject UImanage;
    [SerializeField] GameObject PlayerUI;
    [SerializeField] InputField joinCode;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
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
            PlayerUI.SetActive(true);

            Logger.Instance.LogInfo("Host Connected");
            
            

                
        });

        serverButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            UImanage.SetActive(false);
           
        });

        clientButton.onClick.AddListener(async () =>
        {
            if (RelayManager.instance.IsRelayEnabled && !string.IsNullOrEmpty(joinCode.text))
                await RelayManager.instance.JoinRelay(joinCode.text);

            //FindObjectOfType<PlayerMovement>().ClientHasSpawned();
            NetworkManager.Singleton.StartClient();
            //HealthBar.instance.CheckPlayerJoined();
            UImanage.SetActive(false);
           
            Logger.Instance.LogInfo("Client Connected");
            HealthBar.instance.Condition();

        });

        RestartButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            UImanage.SetActive(true);
            SceneManager.LoadScene(0);
        });
        QuitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

    }
    public void Shutdown()
    {
        
        NetworkManager.Singleton.Shutdown();
        UImanage.SetActive(true);
    }
    public void ShowEndMenu()
    {
        endMenu.SetActive(true);
    }
   
}
