using UnityEngine;
using UnitonConnect.Core;
using TMPro;
using System;
using UnitonConnect.Core.Data;

public class Click : MonoBehaviour
{
    public int targetScore = 10;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI balanceText;
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
        balanceText.text = $"Balance: {Math.Round(tonBalance, 4)} TON";
    }
    public void WalletConnected(WalletConfig wallet)
    {
        sdk.LoadBalance();
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

    public void Receive()
    {
        //thuong ton vao vi nguoi choi
    }    

    private void OnDestroy()
    {
        if (sdk == null) return;
        sdk.OnTonBalanceClaimed -= UpdateBalance;
        sdk.OnWalletConnected -= WalletConnected;
    }
}
