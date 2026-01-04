using UnityEngine;
using System;
using UnitonConnect.Core;
using UnitonConnect.Core.Data;

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }

    public string heart { get; private set; }
    public string laser { get; private set; }

    public int levelDig { get; private set; }
    public int levelHeart { get; private set; }
    public int levelOre { get; private set; }

    public event Action<string> OnHeartUpdated;
    public event Action<string> OnLaserUpdated;
    public event Action<int> OnLevelDigUpdated;
    public event Action<int> OnLevelHeartUpdated;
    public event Action<int> OnLevelOreUpdated;

    private const string GetHeartMethod = "get_heart";
    private const string GetLaserMethod = "get_laser";
    private const string GetLevelDigMethod = "get_levelDig";
    private const string GetLevelHeartMethod = "get_levelHeart";
    private const string GetLevelOreMethod = "get_levelOre";

    private void Start()
    {
        if (UnitonConnectSDK.Instance == null)
        {
            Debug.Log("UnitonConnectSDK null");
            //UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            return;
        }

        FetchHeartCount();
        FetchLaserCount();

        FetchLevelDig();
        FetchLevelHeart();
        FetchLevelOre();
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
    public void FetchLevelDig()
    {
        if (UnitonConnectSDK.Instance.Wallet == null) return;

        string playerAddress = UnitonConnectSDK.Instance.Wallet.ToString();
        UnitonConnectSDK.Instance.GetPlayerStat(GetLevelDigMethod, playerAddress, (result) =>
        {
            Debug.Log($"Level Laser updated: {result}");
            int.TryParse(result, out int level);
            levelDig = level;
            OnLevelDigUpdated?.Invoke(levelDig);
        });
    }
    public void FetchLevelHeart()
    {
        if (UnitonConnectSDK.Instance.Wallet == null) return;

        string playerAddress = UnitonConnectSDK.Instance.Wallet.ToString();
        UnitonConnectSDK.Instance.GetPlayerStat(GetLevelHeartMethod, playerAddress, (result) =>
        {
            Debug.Log($"Level Heart updated: {result}");
            int.TryParse(result, out int level);
            levelHeart = level;
            OnLevelHeartUpdated?.Invoke(levelHeart);
        });
    }
    public void FetchLevelOre()
    {
        if (UnitonConnectSDK.Instance.Wallet == null) return;

        string playerAddress = UnitonConnectSDK.Instance.Wallet.ToString();
        UnitonConnectSDK.Instance.GetPlayerStat(GetLevelOreMethod, playerAddress, (result) =>
        {
            Debug.Log($"Level Ore updated: {result}");
            int.TryParse(result, out int level);
            levelOre = level;
            OnLevelOreUpdated?.Invoke(levelOre);
        });
    }
}