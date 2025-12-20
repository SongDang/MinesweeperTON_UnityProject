using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        priceText.text = data.price.ToString();
        infoText.text = data.info;

        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(() => _onPurchaseClicked?.Invoke(_data));
    }
}
