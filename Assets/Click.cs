using UnityEngine;
using UnitonConnect.Core;
using TMPro;
using System;
using UnitonConnect.Core.Data;
using UnitonConnect.Core.Utils.Debugging;

public class Click : MonoBehaviour
{
    public int targetScore = 10;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI totalScoreText;
    public GameObject panelReward;

    private UnitonConnectSDK sdk;
    private int score = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sdk = UnitonConnectSDK.Instance;
        sdk.OnTonBalanceClaimed += UpdateBalance;
        sdk.OnWalletConnected += WalletConnected;

        if (sdk.IsWalletConnected)
            sdk.LoadBalance(); // chi goi khi da ket 
        else
            balanceText.text = "Wallet not connected";

        score = 0;
        panelReward.SetActive(false);
    }

    public void UpdateBalance(decimal tonBalance)
    {
        UnitonConnectLogger.Log("Update Player Balance");
        balanceText.text = $"Balance: {Math.Round(tonBalance, 4)} TON";
    }
    public void WalletConnected(WalletConfig wallet)
    {
        sdk.LoadBalance();
        UpdateTotalScore(wallet.Address);
    }
    public void Receive()
    {
        //thuong ton vao vi nguoi choi
    }
    public void UpdateTotalScore(string playerAddress)
    {
        UnitonConnectLogger.Log("Update Player Total Score - START");
        Debug.Log("UpdateTotalScore called with address: " + playerAddress);

        try
        {
            sdk.GetPlayerTotalScore(playerAddress,
                /*(result) =>
                {
                    Debug.Log("Callback received!");
                    if (result == null)
                    {
                        UnitonConnectLogger.Log("Player score NULL");
                        Debug.Log("Result is NULL");
                    }
                    else
                    {
                        UnitonConnectLogger.Log("get Player score successfully");
                        Debug.Log("Result: " + result);
                        totalScoreText.text = result.ToString();
                    }
                }*/HandleScoreResult);

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
    }
    public void AddScore()
    {
        score++;
        scoreText.text = "Score: " + score.ToString();

        if(score>=targetScore)
        {
            Win();
        }
    }
    public void Win()
    {
        panelReward.SetActive(true);
        score = 0;
        scoreText.text = "Score: " + score.ToString();
    }
    private void OnDestroy()
    {
        if (sdk == null) return;
        sdk.OnTonBalanceClaimed -= UpdateBalance;
        sdk.OnWalletConnected -= WalletConnected;
    }
}
