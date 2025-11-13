using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Kéo Prefab InventorySlotTemplate vào đây")]
    public GameObject inventorySlotPrefab;
    [Tooltip("Kéo GameObject InventoryGrid vào đây")]
    public Transform inventoryGrid;

    // Giả sử mỗi lần mua chỉ thêm 1 item
    public void AddItemToInventory(ItemData itemData)
    {
        // 1. Tạo một ô đồ mới từ prefab
        GameObject slotObject = Instantiate(inventorySlotPrefab, inventoryGrid);

        // 2. Tìm các component con bên trong ô đồ
        Image itemIcon = slotObject.GetComponent<Image>();
        // TextMeshProUGUI itemCountText = slotObject.transform.Find("ItemCountText").GetComponent<TextMeshProUGUI>();

        
            // Cập nhật icon
            if (itemData.type == ItemType.Weapon && itemData.weaponData != null && itemData.weaponData.weaponIcon != null)
            {
                itemIcon.sprite = itemData.weaponData.weaponIcon;
                itemIcon.color = Color.white;
            } else if(itemData.sprite != null)
            {
                itemIcon.sprite = itemData.sprite;
                itemIcon.color = Color.white;
            }else
        {
            Debug.LogWarning("ItemData hoặc sprite của nó bị null!");
        }
            itemIcon.enabled = true; // Kích hoạt hiển thị icon
        }

        //// 3. Cập nhật thông tin cho ô đồ
        //if (itemIcon != null)
        //{
        //    if (itemData != null && itemData.sprite != null) // Đảm bảo itemData và icon của nó không null
        //    {
        //        Debug.Log("Tìm thấy Icon, đang gán sprite: " + itemData.sprite.name); // THÊM DEBUG.LOG
        //        itemIcon.sprite = itemData.sprite;
        //        itemIcon.enabled = true; // <-- Dòng này cực kỳ quan trọng
        //    }else if(itemData.type == ItemType.Weapon && itemData.weaponData != null && itemData.weaponData.weaponIcon != null)
        //    {
        //        Debug.Log("Tìm thấy Icon vũ khí, đang gán sprite: " + itemData.weaponData.weaponIcon.name); // THÊM DEBUG.LOG
        //        itemIcon.sprite = itemData.weaponData.weaponIcon;
        //        itemIcon.enabled = true; // <-- Dòng này cực kỳ quan trọng
        //    }
        //    else
        //    {
        //        Debug.LogWarning("ItemData hoặc sprite của nó bị null!");
        //    }
        //}
        //else
        //{
        //    Debug.LogError("KHÔNG TÌM THẤY GAMEOBJECT CON TÊN LÀ 'ItemIcon' BÊN TRONG PREFAB!");
        //}

        // itemCountText.text = "x1";
        // itemCountText.gameObject.SetActive(true);
    }

