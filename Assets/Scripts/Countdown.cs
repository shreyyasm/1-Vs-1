using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Countdown : MonoBehaviour
{
    public static Countdown Instance;
    public int countdownTime;
    public TextMeshProUGUI countdownDisplay;


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        StartCoroutine(CountdownToStart());
        SoundManager.Manager.PlaySFX(SoundManager.Countdown);
    }
    
    IEnumerator CountdownToStart()
    {

        while (countdownTime > 0)
        {

            countdownDisplay.text = countdownTime.ToString();
            yield return new WaitForSeconds(1.2f);

            countdownTime--;
        }

        countdownDisplay.text = "Fight!";
        //begin game
        //HorseBetNumbers.instance.StartGame();
        //CameraFollow.instance.RaceBegin();
        SoundManager.Manager.PlayGamePlayMusic();
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
    
}
