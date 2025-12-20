using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemData", menuName = "Scriptable Objects/ShopItemData")]
public class ShopItemData : ScriptableObject
{
    public Sprite icon;
    public string id;
    public string itemName;         
    public float price;
    [TextArea] public string info; 
}
