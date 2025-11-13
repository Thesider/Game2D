using UnityEngine;
using UnityEngine.InputSystem.Controls;


// Enum để định nghĩa các loại item
public enum ItemType
{
    Health,
    Armor,
    SpeedBoost,
    Invincibility,
    Treasure,
    BlindBox,
    Weapon
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public Sprite sprite;
    public int point;
    public int dropChance;
    public ItemType type;
    public string itemName;
    [Header("Shop Information")]
    [Tooltip("Giá bán của vật phẩm trong shop. Đặt là 0 nếu không bán.")]
    public int price;

    [Header("Values")]
    [Tooltip("Dùng cho Health, Armor, Treasure")]
    public int amount; // Một biến giá trị chung

    [Tooltip("Dùng cho SpeedBoost, Invincibility")]
    public float duration; // Một biến thời gian chung

    [Header("Weapon Config")]
    [Tooltip("Chỉ điền vào đây nếu 'Type' là Weapon")]
    public WeaponData weaponData;
}
