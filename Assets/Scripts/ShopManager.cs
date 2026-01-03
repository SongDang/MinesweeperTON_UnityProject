using System.Collections.Generic;
using UnityEngine;
using UnitonConnect.Core;

public class ShopManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<ShopItemData> shopItems; 
    [SerializeField] private ShopItemView shopItemPrefab;      
    [SerializeField] private Transform itemsContainer;     

    void Start()
    {
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
        if(!UnitonConnectSDK.Instance.IsWalletConnected)
        {
            Debug.Log("Purchased failed, wallet not connected");
            return;
        }

        Debug.Log($"Buy: {item.itemName} with {item.price} TON");

        if(item.name=="heart")
        {
            string jsonParams = "{\"qty\": 1}"; //0.1 ton
            UnitonConnectSDK.Instance.SendSmartContractTransaction("buy_heart", jsonParams);
        }
        else
        {
            string jsonParams = "{\"qty\": 1}"; //0.1 ton
            UnitonConnectSDK.Instance.SendSmartContractTransaction("buy_laser", jsonParams);
        }
    }

    public void OpenShop()
    {
        gameObject.SetActive(true);
    }
    public void CloseShop()
    {
        gameObject.SetActive(false);
    }
}
