using System.Collections.Generic;
using UnityEngine;
using UnitonConnect.Core;
using TMPro;
using UnitonConnect.Core.Utils.Debugging;
using System;

public class ShopManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<ShopItemData> shopItems; 
    [SerializeField] private ShopItemView shopItemPrefab;      
    [SerializeField] private Transform itemsContainer;     
    [SerializeField] private TextMeshProUGUI balanceText;

    private UnitonConnectSDK sdk;
    void Start()
    {
        sdk = UnitonConnectSDK.Instance;
        if (sdk == null)
        {
            UnitonConnectLogger.Log("Shop get SDK null");
            return;
        }

        sdk.OnTonBalanceClaimed += UpdateBalanceText;

        GenerateShopItems();
    }

    void GenerateShopItems()
    {
        //delete old items
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var data in shopItems)
        {
            ShopItemView newItem = Instantiate(shopItemPrefab, itemsContainer);
            newItem.Setup(data, OnItemPurchased);
        }
    }

    void OnItemPurchased(ShopItemData item)
    {
        if(!sdk.IsWalletConnected)
        {
            Debug.Log("Purchased failed, wallet not connected");
            return;
        }

        if(item.itemName=="heart") //"itemName" not "name"
        {
            string jsonParams = "{\"qty\": 1}"; //1 item
            decimal itemPrice = item.price;
            sdk.SendSmartContractTransaction("buy_heart", jsonParams, itemPrice);

            Debug.Log($"Buy: heart with {item.price} TON");
        }
        else
        {
            string jsonParams = "{\"qty\": 1}"; //1 item
            decimal itemPrice = item.price;
            sdk.SendSmartContractTransaction("buy_laser", jsonParams, itemPrice);

            Debug.Log($"Buy: laser with {item.price} TON");
        }
    }

    public void OpenShop()
    {
        gameObject.SetActive(true);

        UpdateBalanceText(sdk.TonBalance);
    }
    public void CloseShop()
    {
        gameObject.SetActive(false);
    }
    public void UpdateBalanceText(decimal tonBalance)
    {
        if(balanceText==null)
        {
            UnitonConnectLogger.Log("Shop Balance text null");
        }

        UnitonConnectLogger.Log("Update Shop Balance Text");
        balanceText.text = $"{Math.Round(tonBalance, 4)}";
    }
    private void OnDestroy()
    {
        if (sdk != null)
        {
            sdk.OnTonBalanceClaimed -= UpdateBalanceText;
        }
    }
}
