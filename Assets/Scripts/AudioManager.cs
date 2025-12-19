using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    private static AudioManager _instance;
    public static AudioManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }
        else
            _instance = this;

    }

    #endregion Singleton

    public AudioSource _audiosourceBG;
    public AudioSource _audiosourceFX;
    public AudioSource _audiosourceOre;
    public AudioSource _audiosourceWin;
    
    public AudioClip bgmusic;
    public AudioClip digsound;
    public AudioClip rocksmash;
    public AudioClip ore;
    public AudioClip explode;
    public AudioClip sweeper;
    public AudioClip win;
    public AudioClip rare;

    void Start()
    {
        _audiosourceBG.clip = bgmusic;
        _audiosourceBG.Play();      
    }

    
    void Update()
    {
        
    }

    public void DigSound()
    {
        _audiosourceWin.clip = digsound;
        _audiosourceWin.Play();
    }   

    public void RockSmashSound()
    {
        _audiosourceFX.clip = rocksmash;
        _audiosourceFX.Play();
    }    

    public void OreSound()
    {
        _audiosourceOre.clip = ore;
        _audiosourceOre.Play();
    }    
   
    public void ExplodeSound()
    {
        _audiosourceOre.clip = explode;
        _audiosourceOre.Play(); 
    }

    public void SweeperSound()
    {
        _audiosourceFX.clip = sweeper;
        _audiosourceFX.Play();
    }

    public void WinSound()
    {
        _audiosourceWin.clip = win;
        _audiosourceWin.Play();
    }

    public void RareOreSound()
    {
        _audiosourceWin.clip = rare;
        _audiosourceWin.Play();
    }


}
