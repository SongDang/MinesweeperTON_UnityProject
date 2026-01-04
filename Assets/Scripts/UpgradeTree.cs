using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnitonConnect.Core;
using UnitonConnect.Core.Utils.Debugging;
using System.Collections.Generic;

public class UpgradeTree : MonoBehaviour
{
    public enum UpgradeType
    {
        None,
        DigSpeed,
        MaxHeart,
        OreLuck
    }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI textBalance; 
    [SerializeField] private TextMeshProUGUI textInfo;    
    [SerializeField] private Button btnConfirm;           

    [Header("Upgrade Buttons")]
    [SerializeField] private Button[] btnDigs;   
    [SerializeField] private Button[] btnHearts;
    [SerializeField] private Button[] btnOres;

    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;
    private Button currentSelectedBtn;

    private UpgradeType selectedType = UpgradeType.None;
    private int selectedLevelTarget = 0; 
    private decimal currentCost = 0;     

    private Dictionary<UpgradeType, decimal> cachedPrices = new Dictionary<UpgradeType, decimal>();
    private PlayerStatsManager playerStatsManager;
    void Start()
    {
        if (UnitonConnectSDK.Instance != null)
        {
            UpdateBalanceUI(UnitonConnectSDK.Instance.TonBalance);
            UnitonConnectSDK.Instance.OnTonBalanceClaimed += UpdateBalanceUI;
        }

        playerStatsManager = PlayerStatsManager.Instance;
        if (playerStatsManager == null)
        {
            UnitonConnectLogger.Log("Upgrade tree get playerStatsManager null");
            return;
        }

        SetupButtonListeners();

        FetchUpgradePrices();

        UpdateTreeUI();
        SubscribeEvents();
    }

    public void OnSelectUpgrade(int typeInt, int targetLevelIndex)
    {
        selectedType = (UpgradeType)(typeInt + 1); 
        selectedLevelTarget = targetLevelIndex;

        if (cachedPrices.TryGetValue(selectedType, out decimal price))
        {
            currentCost = price;
        }
        else
        {
            currentCost = 0.5m; //default
        }

        UpdateInfoText();
        btnConfirm.interactable = true;
    }
    private void SetupButtonListeners()
    {
        for (int i = 0; i < btnDigs.Length; i++)
        {
            int index = i;
            btnDigs[i].onClick.AddListener(() => OnSelectButton(btnDigs[index], 0, index));
        }

        for (int i = 0; i < btnHearts.Length; i++)
        {
            int index = i;
            btnHearts[i].onClick.AddListener(() => OnSelectButton(btnHearts[index], 1, index));
        }

        for (int i = 0; i < btnOres.Length; i++)
        {
            int index = i;
            btnOres[i].onClick.AddListener(() => OnSelectButton(btnOres[index], 2, index));
        }
    }
    public void OnSelectButton(Button btnClicked, int typeInt, int targetLevelIndex)
    {
        if (currentSelectedBtn != null && currentSelectedBtn != btnClicked)
        {
            if (currentSelectedBtn.image.color == selectedColor)
                currentSelectedBtn.image.color = normalColor;
        }

        currentSelectedBtn = btnClicked;
        if (currentSelectedBtn.image.color != Color.green)
            currentSelectedBtn.image.color = selectedColor;

        OnSelectUpgrade(typeInt, targetLevelIndex);
    }

    private void UpdateInfoText()
    {
        string info = "";
        string costStr = $"{currentCost:0.##} TON";

        switch (selectedType)
        {
            case UpgradeType.DigSpeed:
                if (selectedLevelTarget == 0) info = $"Dig Time: 2s -> 1.5s\nCost: {costStr}";
                else info = $"Dig Time: 1.5s -> 1s\nCost: {costStr}";
                break;
            case UpgradeType.MaxHeart:
                info = $"Max Hearts: {3 + selectedLevelTarget} -> {4 + selectedLevelTarget}\nCost: {costStr}";
                break;
            case UpgradeType.OreLuck:
                if (selectedLevelTarget == 0) info = $"Gold Chance +10%\nCost: {costStr}";
                else info = $"Gold Chance +5%, Diamond +3%\nCost: {costStr}";
                break;
        }
        textInfo.text = info;
    }

    public void UpdateTreeUI()
    {
        UpdateDigUI(playerStatsManager.levelDig);
        UpdateHeartUI(playerStatsManager.levelHeart);
        UpdateOreUI(playerStatsManager.levelOre);
    }

    private void SubscribeEvents()
    {
        playerStatsManager.OnLevelDigUpdated += UpdateDigUI;
        playerStatsManager.OnLevelHeartUpdated += UpdateHeartUI;
        playerStatsManager.OnLevelOreUpdated += UpdateOreUI;
    }

    public void UpdateDigUI(int level)
    {
        SetButtonState(btnDigs, level);
    }
    public void UpdateHeartUI(int level)
    {
        SetButtonState(btnHearts, level);
    }
    public void UpdateOreUI(int level)
    {
        SetButtonState(btnOres, level);
    }

    private void SetButtonState(Button[] buttons, int currentLevel)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                bool canUpgrade = (currentLevel == (i + 1));
                bool isUpgraded = (currentLevel > (i + 1));

                buttons[i].interactable = canUpgrade;

                //change color
                if (isUpgraded)
                    buttons[i].image.color = Color.green;
                else if (canUpgrade)
                    buttons[i].image.color = normalColor;
                else
                    buttons[i].image.color = Color.gray;
            }
        }
    }

    private void UpdateBalanceUI(decimal balance)
    {
        if (textBalance != null && UnitonConnectSDK.Instance != null)
        {
            textBalance.text = $"{balance:0.####} TON";
        }
    }

    private void FetchUpgradePrices()
    {
        UnitonConnectSDK.Instance.GetGameData("get_price_upgrade", (result) =>
        {
            if (decimal.TryParse(result, out decimal nanoPrice))
            {
                decimal price = nanoPrice / 1_000_000_000m;
                cachedPrices[UpgradeType.DigSpeed] = price;
                cachedPrices[UpgradeType.MaxHeart] = price; 
                cachedPrices[UpgradeType.OreLuck] = price;
            }
            else
            {
                UnitonConnectLogger.Log("Cannot parse upgrade price");
            }
        });
    }

    public void ConfirmUpgrade()
    {
        if (selectedType == UpgradeType.None) return;

        if (UnitonConnectSDK.Instance.TonBalance < currentCost)
        {
            textInfo.text = "Not enough TON!";
            return;
        }

        string jsonParams = $"{{ \"upgradeType\": {(int)selectedType} }}";

        UnitonConnectSDK.Instance.OnTonTransactionSended += OnUpgradeSuccess;
        UnitonConnectSDK.Instance.OnTonTransactionSendFailed += OnUpgradeFailed;

        btnConfirm.interactable = false;

        UnitonConnectSDK.Instance.SendSmartContractTransaction("buy_upgrade", jsonParams, currentCost);
    }

    private void OnUpgradeFailed(string error)
    {
        UnitonConnectLogger.Log("Upgrade failed, error: " + error);
        CleanupTransaction();
    }

    private void OnUpgradeSuccess(string hash)
    {
        UnitonConnectLogger.Log("Upgrade success, transaction hash: " + hash);
        AudioManager.Instance.WinSound();

        ApplyUpgradeEffect();

        //Game.Instance.SaveData();
        UpdateTreeUI();

        CleanupTransaction();
    }

    private void CleanupTransaction()
    {
        btnConfirm.interactable = true;
        if (UnitonConnectSDK.Instance != null)
        {
            UnitonConnectSDK.Instance.OnTonTransactionSended -= OnUpgradeSuccess;
            UnitonConnectSDK.Instance.OnTonTransactionSendFailed -= OnUpgradeFailed;
        }
    }

    private void ApplyUpgradeEffect()
    {
        var userData = Game.Instance._userDatas;

        switch (selectedType)
        {
            case UpgradeType.DigSpeed:
                userData.timedig -= 0.5f;
                // playerStatsManager.ForceUpdateLevel(UpgradeType.DigSpeed, selectedLevelTarget + 2); 
                break;

            case UpgradeType.MaxHeart:
                userData.maxheart += 1;
                userData.heart += 1;
                break;

            case UpgradeType.OreLuck:
                if (selectedLevelTarget == 0) userData.probalitygold += 10;
                else
                {
                    userData.probalitygold += 5;
                    userData.probalitydiamond += 3;
                }
                break;
        }
    }
    private void OnDestroy()
    {
        if (UnitonConnectSDK.Instance != null)
        {
            UnitonConnectSDK.Instance.OnTonBalanceClaimed -= UpdateBalanceUI;
        }

        if (playerStatsManager != null)
        {
            playerStatsManager.OnLevelDigUpdated -= UpdateDigUI;
            playerStatsManager.OnLevelHeartUpdated -= UpdateHeartUI;
            playerStatsManager.OnLevelOreUpdated -= UpdateOreUI;
        }

        CleanupTransaction();
    }
}