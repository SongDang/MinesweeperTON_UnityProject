using UnityEngine;
using UnitonConnect.Core;
using UnitonConnect.Core.Utils.Debugging;

public class HeartReward : MonoBehaviour
{
    public void GetHeartReward()
    {
        Debug.Log("Get heart reward");
        string jsonParams = "{\"amountHeart\": 1}"; //dummy
        UnitonConnectSDK.Instance.SendSmartContractTransaction(HandleGetHeartRewardResult, "heart_reward", jsonParams);
    }
    private void HandleGetHeartRewardResult(bool isSuccess)
    {
        if (isSuccess)
        {
            int heartCount = 2 + PlayerStatsManager.Instance.levelHeart; //lv0: 2, lv1: 3, lv2: 4
            PlayerStatsManager.Instance.AddHeart(heartCount);
            UnitonConnectLogger.Log($"Get heart reward success, heart + {heartCount}");
        }
        else
        {
            UnitonConnectLogger.Log("Get heart reward failed");
        }
    }
}
