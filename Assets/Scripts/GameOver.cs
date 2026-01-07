using System.Collections;
using System.Collections.Generic;
using UnitonConnect.Core;
using UnitonConnect.Core.Utils.Debugging;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject shop;
    public void PlayAgain()
    {
        if(Game.Instance.OutOfHeartScreen.activeSelf == false)
        {
            if (/*Game.Instance._userDatas.heart*/PlayerStatsManager.Instance.heart > 0)
            {
                StartLevel();
            }
            else
            {
                Game.Instance.OutOfHeartScreen.SetActive(true);
            }
        }  
        else
        {
            shop.SetActive(true);
        }    
        /*else
        {
            if(Game.Instance._userDatas.diamond >= 1)
            {
                Game.Instance.NewGame();
                Game.Instance.OutOfHeartScreen.SetActive(false);
                gameObject.SetActive(false);
                Game.Instance._userDatas.diamond--;
                Game.Instance._userDatas.heart++;
                Game.Instance.SaveData();
            }   
        }*/    
    }
    
    public void StartLevel()
    {
        if (!UnitonConnectSDK.Instance.IsWalletConnected)
        {
            UnitonConnectLogger.Log("Wallet is not connected");
            return;
        }

        if (PlayerStatsManager.Instance.heart <= 0)
        {
            UnitonConnectLogger.Log("Not enough heart");
            //pop up
            return;
        }

        string jsonParams = "{\"qty\": 1}"; //1 item
        UnitonConnectSDK.Instance.SendSmartContractTransaction(HandleUseHeartResult, "use_heart", jsonParams);

    }
    private void HandleUseHeartResult(bool isSuccess)
    {
        if (isSuccess)
        {
            PlayerStatsManager.Instance.AddHeart(-1);
            UnitonConnectLogger.Log("Try again use heart success, heart - 1");
            SceneManager.LoadScene("GamePlay");
        }
        else
        {
            UnitonConnectLogger.Log("Try again use heart failed");
        }
    }
    public void NextLevel()
    {
        Game.Instance.SetLevel();
        Game.Instance.NewGame();
        gameObject.SetActive(false);
    }    


}
