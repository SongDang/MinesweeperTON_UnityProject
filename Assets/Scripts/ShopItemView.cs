using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnitonConnect.Core;

public class ShopItemView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TextMeshProUGUI infoText;

    private ShopItemData _data;
    private Action<ShopItemData> _onPurchaseClicked;

    public void Setup(ShopItemData data, Action<ShopItemData> onClickCallback)
    {
        _data = data;
        _onPurchaseClicked = onClickCallback;

        iconImage.sprite = data.icon;
        nameText.text = data.itemName;

        infoText.text = data.info;

        //priceText.text = data.price.ToString();
        //FetchItemPrice(data);
        if (data.itemName == "heart")
        {
            _data.price = GameStatsManager.Instance.PriceHeart;
        }
        else
        {
            _data.price = GameStatsManager.Instance.PriceLaser;
        }

        if (priceText != null)
        {
            priceText.text = $"{_data.price:0.##}";
        }

        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(() => _onPurchaseClicked?.Invoke(_data));
    }
}
