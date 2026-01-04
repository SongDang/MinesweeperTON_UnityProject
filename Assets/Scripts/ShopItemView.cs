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
        FetchItemPrice(data);

        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(() => _onPurchaseClicked?.Invoke(_data));
    }

    public void FetchItemPrice(ShopItemData data)
    {
        if(data.itemName == "heart")
        {
            UnitonConnectSDK.Instance.GetGameData("get_price_heart", (result) =>
            {
                Debug.Log($"Heart price updated: {result}");

                SetPrice(result);
            });
        }
        else
        {
            UnitonConnectSDK.Instance.GetGameData("get_price_laser", (result) =>
            {
                Debug.Log($"Laser price updated: {result}");

                SetPrice(result);
            });
        }

      
    }
    private void SetPrice(string result)
    {
        if (decimal.TryParse(result, out decimal nanoPrice))
        {
            decimal realPrice = nanoPrice / 1_000_000_000m;

            _data.price = realPrice;

            if (priceText != null)
                priceText.text = $"{realPrice:0.##}";
        }
        else
        {
            if (priceText != null) priceText.text = "Error";
        }
    }    
}
