using UnityEngine;
using UnitonConnect.Core;
using UnitonConnect.Core.Utils.Debugging;
using TMPro;
using System;
using System.Collections;

public class LevelSelectionManager : MonoBehaviour
{
    public GameObject failedText;
    public GameObject Howtoplay;

    public void OpenHowtoplay()
    {
        Howtoplay.SetActive(true);
    }
    public void CloseHowtoplay()
    {
        Howtoplay.SetActive(false);
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

        //loading

        //UnitonConnectSDK.Instance.OnTonTransactionSended += OnTxSuccess;
        //UnitonConnectSDK.Instance.OnTonTransactionSendFailed += OnTxFailed;

        string jsonParams = "{\"qty\": 1}"; //1 item
        UnitonConnectSDK.Instance.SendSmartContractTransaction(HandleUseHeartResult, "use_heart", jsonParams);

    }
    private void HandleUseHeartResult(bool isSuccess)
    {
        if (isSuccess)
        {
            PlayerStatsManager.Instance.AddHeart(-1);
            UnitonConnectLogger.Log("Use heart success, heart - 1");
            UnityEngine.SceneManagement.SceneManager.LoadScene("GamePlay");
        }
        else
        {
            UnitonConnectLogger.Log("Use heart failed");
            StartCoroutine(ShowText(3f));
        }
    }
    
    IEnumerator ShowText(float sec)
    {
        failedText.SetActive(true);            
        yield return new WaitForSeconds(sec);    
        failedText.SetActive(false);          
    }

    /*private void OnTxFailed(string errorMsg)
    {
        UnitonConnectLogger.LogError("Start level failed: " + errorMsg);

        Cleanup();
    }

    private void OnTxSuccess(string transactionHash)
    {
        UnitonConnectLogger.Log("Start level success, hash: " + transactionHash);

        Cleanup();

        //PlayerStatsManager.Instance.heart--; 
        UnityEngine.SceneManagement.SceneManager.LoadScene("GamePlay");
    }

    private void Cleanup()
    {
        //cancel loading pop up;

        if (UnitonConnectSDK.Instance != null)
        {
            UnitonConnectSDK.Instance.OnTonTransactionSended -= OnTxSuccess;
            UnitonConnectSDK.Instance.OnTonTransactionSendFailed -= OnTxFailed;
        }
    }

    private void OnDestroy()
    {
        Cleanup();
    }*/
}
