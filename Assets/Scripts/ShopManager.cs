using System.Collections.Generic;
using UnityEngine;

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
        Debug.Log($"Buy: {item.itemName} with {item.price} TON");
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
