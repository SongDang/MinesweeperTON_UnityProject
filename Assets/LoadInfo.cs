using TMPro;
using UnitonConnect.Core;
using UnitonConnect.Core.Utils.Debugging;
using UnityEngine;
using UnitonConnect.Core.Data;
using System;

public class LoadInfo : MonoBehaviour
{
    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI addressText;

    private UnitonConnectSDK sdk;

    void Start()
    {
        UnitonConnectLogger.Log("LoadInfo start");
        sdk = UnitonConnectSDK.Instance;

        if(sdk==null)
        {
            UnitonConnectLogger.Log("SDK null");
            return;
        }

        sdk.OnTonBalanceClaimed += UpdateBalance;
        sdk.OnWalletConnected += WalletConnected;

        if (sdk.IsWalletConnected)
        {
            sdk.LoadBalance();
            UpdateAddress(sdk.Wallet.ToString());
        }
        else
            balanceText.text = "Wallet not connected";

        UnitonConnectLogger.Log("LoadInfo start end");
    }

    public void UpdateBalance(decimal tonBalance)
    {
        UnitonConnectLogger.Log("Update Player Balance");
        balanceText.text = $"Balance: {Math.Round(tonBalance, 4)} TON";
    }
    public void WalletConnected(WalletConfig wallet)
    {
        UnitonConnectLogger.Log("LoadInfo walletconnected");
        try
        {
            sdk.LoadBalance();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[LoadInfo] error when call sdk.LoadBalance: {ex.Message}\nStack: {ex.StackTrace}");
        }

        try
        {
            UpdateAddress(wallet.Address);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[LoadInfo] error when call UpdateTotalScore: {ex.Message}\nStack: {ex.StackTrace}");
        }
    }
    public void UpdateAddress(string playerAddress)
    {
        UnitonConnectLogger.Log("Update Player Address: "+ playerAddress);

        string shortWalletAddress = "";
        if (!string.IsNullOrEmpty(playerAddress) && playerAddress.Length > 10)
        {
            shortWalletAddress = playerAddress.Substring(0, 6) + "..." +
                                playerAddress.Substring(playerAddress.Length - 4);
        }

        addressText.text = shortWalletAddress;
    }
    private void OnDestroy()
    {
        if (sdk == null) return;
        sdk.OnTonBalanceClaimed -= UpdateBalance;
        sdk.OnWalletConnected -= WalletConnected;
    }

    /*public void UpdateTotalScore(string playerAddress)
    {
        UnitonConnectLogger.Log("Update Player Total Score - START");
        Debug.Log("UpdateTotalScore called with address: " + playerAddress);

        try
        {
            sdk.GetPlayerTotalScore(playerAddress, HandleScoreResult);

            UnitonConnectLogger.Log("Update Player Total Score - AFTER GetPlayerTotalScore call");
            Debug.Log("After GetPlayerTotalScore call");
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception: " + ex.Message);
        }
    }
    private void HandleScoreResult(string result)
    {
        if (result == null)
        {
            UnitonConnectLogger.Log("Player score NULL");
        }
        else
        {
            UnitonConnectLogger.Log("get Player score successfully");
            totalScoreText.text = result.ToString();
        }
    }*/
}
