using UnityEngine;
using UnitonConnect.Core;

public class HeartReward : MonoBehaviour
{
    public void GetHeartReward()
    {
        Debug.Log("Get heart reward");
        string jsonParams = "{\"amountHeart\": 1}"; //dummy
        UnitonConnectSDK.Instance.SendSmartContractTransaction("heart_reward", jsonParams);
    }    
}
