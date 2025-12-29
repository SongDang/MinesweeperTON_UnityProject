using UnityEngine;
using UnitonConnect.Core;

public class SellOre : MonoBehaviour
{
    public void Sell()
    {
        Debug.Log("Sell ore");
        string jsonParams = "{\"amountReward\": 100000000}"; //0.1 ton
        UnitonConnectSDK.Instance.SendSmartContractTransaction("claim_reward", jsonParams);
    }    
}
