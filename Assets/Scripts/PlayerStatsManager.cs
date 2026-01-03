using UnityEngine;
using System;
using UnitonConnect.Core;
using UnitonConnect.Core.Data;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }

    public string heart { get; private set; }
    public string laser { get; private set; }

    public event Action<string> OnHeartUpdated;
    public event Action<string> OnLaserUpdated;

    private const string GetHeartMethod = "get_heart";
    private const string GetLaserMethod = "get_laser";

    private void Start()
    {
        if (UnitonConnectSDK.Instance == null)
        {
            Debug.Log("UnitonConnectSDK null");
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            return;
        }

        FetchHeartCount();
        FetchLaserCount();
    }

    private void Awake()
    {
        // Setup Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FetchPlayerStats(WalletConfig wallet)
    {
        FetchHeartCount();
        FetchLaserCount();
    }
    public void FetchHeartCount()
    {
        if (!UnitonConnectSDK.Instance.IsWalletConnected) return;

        string playerAddress = UnitonConnectSDK.Instance.Wallet.ToString();
        UnitonConnectSDK.Instance.GetPlayerStat(GetHeartMethod, playerAddress, (result) =>
        {
            Debug.Log($"Heart updated: {result}");
            heart = result;
            OnHeartUpdated?.Invoke(heart);
        });
    }
    public void FetchLaserCount()
    {
        if (UnitonConnectSDK.Instance.Wallet == null) return;

        string playerAddress = UnitonConnectSDK.Instance.Wallet.ToString();
        UnitonConnectSDK.Instance.GetPlayerStat(GetLaserMethod, playerAddress, (result) =>
        {
            Debug.Log($"Laser updated: {result}");

            laser = result;
            OnLaserUpdated?.Invoke(laser);
        });
    }
}