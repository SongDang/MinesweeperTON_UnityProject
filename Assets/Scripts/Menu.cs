using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitonConnect.Core;

public class Menu : MonoBehaviour
{
    public AudioSource _audiosourceBG;
    public AudioClip bgmenu;
    void Start()
    {
        _audiosourceBG.clip = bgmenu;
        _audiosourceBG.Play();
    }

    public GameObject Howtoplay;

    public void OpenHowtoplay()
    {
        Howtoplay.SetActive(true);
    }

    public void CloseHowtoplay()
    {
        Howtoplay.SetActive(false);
    }

    public void Playnow()
    {
        if(UnitonConnectSDK.Instance.IsWalletConnected)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GamePlay");
        }
        else
        {
            Debug.Log("Connect your wallet to start playing");
        }
    }

}
