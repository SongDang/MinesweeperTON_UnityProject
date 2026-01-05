using UnityEngine;
using System;
using UnitonConnect.Core;
using UnitonConnect.Core.Data;
using System.Collections;

[Serializable]
public class PlayerInfo
{
    public int heart;
    public int laser;
    public int lastTimeReceiveHeart;
    public int remainingHeartCooldown;
    public int levelHeart;
    public int levelDig;
    public int levelOre;
}

public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }

    public int heart { get; private set; }
    public int laser { get; private set; }
    public int lastTimeReceiveHeart { get; private set; }
    public int remainingHeartCooldown { get; private set; }

    public int levelDig { get; private set; }
    public int levelHeart { get; private set; }
    public int levelOre { get; private set; }

    public event Action<int> OnHeartUpdated;
    public event Action<int> OnLaserUpdated;
    public event Action<int> OnLastTimeUpdated;
    public event Action<int> OnRemainingCooldownUpdated;
    public event Action<int> OnLevelDigUpdated;
    public event Action<int> OnLevelHeartUpdated;
    public event Action<int> OnLevelOreUpdated;

    private const string GetHeartMethod = "get_heart";
    private const string GetLaserMethod = "get_laser";
    private const string GetLevelDigMethod = "get_levelDig";
    private const string GetLevelHeartMethod = "get_levelHeart";
    private const string GetLevelOreMethod = "get_levelOre";
    private const string GetPlayerInfo = "get_player_info";

    private void Start()
    {
        if (UnitonConnectSDK.Instance == null)
        {
            Debug.Log("UnitonConnectSDK null");
            //UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            return;
        }

        FetchAllStats();
    }

    private void FetchAllStats()
    {
        //call get_player_info
        if (!UnitonConnectSDK.Instance.IsWalletConnected) return;

        string playerAddress = UnitonConnectSDK.Instance.Wallet.ToString();
        UnitonConnectSDK.Instance.GetPlayerStat(GetPlayerInfo, playerAddress, (jsonResult) =>
        {
            try
            {
                Debug.Log($"[FetchAllStats] Received JSON: {jsonResult}");

                PlayerInfo info = JsonUtility.FromJson<PlayerInfo>(jsonResult);

                Debug.Log($"[FetchAllStats] Parsed:\n" +
                            $"- Heart: {info.heart}\n" +
                            $"- Laser: {info.laser}\n" +
                            $"- LastTime: {info.lastTimeReceiveHeart}\n" +
                            $"- RemainingHeartCooldown: {info.remainingHeartCooldown}\n" +
                            $"- LvlDig: {info.levelDig}\n" +
                            $"- LvlHeart: {info.levelHeart}\n" +
                            $"- LvlOre: {info.levelOre}");

                heart = info.heart;
                laser = info.laser;
                lastTimeReceiveHeart = info.lastTimeReceiveHeart;
                remainingHeartCooldown = info.remainingHeartCooldown;
                levelDig = info.levelDig;
                levelHeart = info.levelHeart;
                levelOre = info.levelOre;

                // Invoke events
                OnHeartUpdated?.Invoke(heart);
                OnLaserUpdated?.Invoke(laser);
                OnLastTimeUpdated?.Invoke(lastTimeReceiveHeart);
                OnRemainingCooldownUpdated?.Invoke(remainingHeartCooldown);
                OnLevelHeartUpdated?.Invoke(levelHeart);
                OnLevelDigUpdated?.Invoke(levelDig);
                OnLevelOreUpdated?.Invoke(levelOre);

                Debug.Log($"[FetchAllStats] All stats updated successfully!");
            }
            catch (Exception e)
            {
                Debug.LogError($"[FetchAllStats] Failed to parse: {e.Message}");

                // Use default values on error
                heart = 0;
                laser = 0;
                lastTimeReceiveHeart = 0;
                remainingHeartCooldown = 0;
                levelDig = 1;
                levelHeart = 1;
                levelOre = 1;

                OnHeartUpdated?.Invoke(heart);
                OnLaserUpdated?.Invoke(laser);
            }
        });
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

            int.TryParse(result, out int resultInt);
            heart = resultInt;
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

            int.TryParse(result, out int resultInt);
            laser = resultInt;
            OnLaserUpdated?.Invoke(laser);
        });
    }
    public void FetchLevelDig()
    {
        if (UnitonConnectSDK.Instance.Wallet == null) return;

        string playerAddress = UnitonConnectSDK.Instance.Wallet.ToString();
        UnitonConnectSDK.Instance.GetPlayerStat(GetLevelDigMethod, playerAddress, (result) =>
        {
            Debug.Log($"Level Dig updated: {result}");
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

    public void AddHeart(int count)
    {
        heart += count;
        OnHeartUpdated?.Invoke(heart);
    }
    public void AddLaser(int amount)
    {
        laser += amount;
        OnLaserUpdated?.Invoke(laser);
    }
    public void SetRemainingHeartCooldown(int seconds)
    {
        remainingHeartCooldown = seconds;
        OnRemainingCooldownUpdated?.Invoke(remainingHeartCooldown);
    }
    public void LevelUpDig()
    {
        levelDig++;
        OnLevelDigUpdated?.Invoke(levelDig);
    }
    public void LevelUpHeart()
    {
        levelHeart++;
        OnLevelHeartUpdated?.Invoke(levelHeart);
    }
    public void LevelUpOre()
    {
        levelOre++;
        OnLevelOreUpdated?.Invoke(levelOre);
    }
}