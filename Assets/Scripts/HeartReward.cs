using UnityEngine;
using UnitonConnect.Core;
using UnitonConnect.Core.Utils.Debugging;

public class HeartReward : MonoBehaviour
{
    public void GetHeartReward()
    {
        Debug.Log("Get heart reward");
        string jsonParams = "{\"amountHeart\": 1}"; //dummy

        UnitonConnectSDK.Instance.OnTonTransactionSended += OnTxSuccess;
        UnitonConnectSDK.Instance.OnTonTransactionSendFailed += OnTxFailed;

        UnitonConnectSDK.Instance.SendSmartContractTransaction("heart_reward", jsonParams);
    }
    private void OnTxFailed(string errorMsg)
    {
        UnitonConnectLogger.LogError("Heart reward failed: " + errorMsg);

        Cleanup();
    }

    private void OnTxSuccess(string transactionHash)
    {
        UnitonConnectLogger.Log("Heart reward success, hash: " + transactionHash);

        int heartCount = 2 + PlayerStatsManager.Instance.levelHeart; //lv0: 2, lv1: 3, lv2: 4
        //PlayerStatsManager.Instance.AddHeart(heartCount);

        Cleanup();
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
}
