using UnityEngine;
using UnitonConnect.Core;
using UnitonConnect.Core.Utils.Debugging;

public class SellOre : MonoBehaviour
{
    public void Sell()
    {
        Debug.Log("Sell ore");
        string jsonParams = "{\"amountReward\": 100000000}"; //0.1 ton
        UnitonConnectSDK.Instance.SendSmartContractTransaction(HandleClaimRewardResult, "claim_reward", jsonParams);
    }

    private void HandleClaimRewardResult(bool isSuccess)
    {
        if (isSuccess)
        {
            UnitonConnectSDK.Instance.LoadBalance();
            UnitonConnectLogger.Log("Claim reward success");
        }
        else
        {
            UnitonConnectLogger.Log("Claim reward failed");
        }
    }
}
