using UnityEngine;
using System;
using UnitonConnect.Core;
using UnitonConnect.Core.Utils.Debugging;

// JsonUtility Unity not support decimal, use string and convert
[Serializable]
public class GameInfoData
{
    public string priceHeart;
    public string priceLaser;
    public string priceGold;
    public string priceDiamond;
    public string priceWin;
    public string priceUpgrade; 
}

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance { get; private set; }

    public decimal PriceHeart { get; private set; }
    public decimal PriceLaser { get; private set; }
    public decimal PriceGold { get; private set; }
    public decimal PriceDiamond { get; private set; }
    public decimal PriceWin { get; private set; }
    public decimal PriceUpgrade { get; private set; }

    public event Action<decimal> OnPriceHeartUpdated;
    public event Action<decimal> OnPriceLaserUpdated;
    public event Action<decimal> OnPriceGoldUpdated;
    public event Action<decimal> OnPriceDiamondUpdated;
    public event Action<decimal> OnPriceWinUpdated;
    public event Action<decimal> OnPriceUpgradeUpdated;

    private const string GetGameInfoMethod = "get_all_prices";

    private const decimal NanoToTon = 1_000_000_000m;

    private void Awake()
    {
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

    private void Start()
    {
        if (UnitonConnectSDK.Instance == null)
        {
            UnitonConnectLogger.LogError("UnitonConnectSDK is null in GameStatsManager");
            return;
        }

        if (UnitonConnectSDK.Instance.IsWalletConnected)
        {
            FetchAllGameStats();
        }
    }

    public void FetchAllGameStats()
    {
        UnitonConnectSDK.Instance.GetGameData(GetGameInfoMethod, (jsonResult) =>
        {
            try
            {
                UnitonConnectLogger.Log($"[GameStats] Received JSON: {jsonResult}");

                GameInfoData data = JsonUtility.FromJson<GameInfoData>(jsonResult);

                // Convert from NanoTON (String/Long) to Decimal (TON)
                PriceHeart = ParseNano(data.priceHeart);
                PriceLaser = ParseNano(data.priceLaser);
                PriceGold = ParseNano(data.priceGold);
                PriceDiamond = ParseNano(data.priceDiamond);
                PriceWin = ParseNano(data.priceWin);
                PriceUpgrade = ParseNano(data.priceUpgrade);

                // update UI
                OnPriceHeartUpdated?.Invoke(PriceHeart);
                OnPriceLaserUpdated?.Invoke(PriceLaser);
                OnPriceGoldUpdated?.Invoke(PriceGold);
                OnPriceDiamondUpdated?.Invoke(PriceDiamond);
                OnPriceWinUpdated?.Invoke(PriceWin);
                OnPriceUpgradeUpdated?.Invoke(PriceUpgrade);

                UnitonConnectLogger.Log("[GameStats] All prices updated successfully!");
            }
            catch (Exception e)
            {
                UnitonConnectLogger.LogError($"[GameStats] Failed to parse: {e.Message}");
                SetDefaultValues();
            }
        });
    }

    private decimal ParseNano(string nanoString)
    {
        if (decimal.TryParse(nanoString, out decimal result))
        {
            return result / NanoToTon;
        }
        return 0;
    }

    private void SetDefaultValues()
    {
        PriceHeart = 0.1m;
        PriceLaser = 0.5m;
        PriceGold = 0.1m;
        PriceDiamond = 0.5m;
        PriceWin = 1.0m;
        PriceUpgrade = 0.5m;
    }
}