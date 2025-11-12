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
        Image itemIcon = slotObject.transform.Find("ItemIcon").GetComponent<Image>();
        // TextMeshProUGUI itemCountText = slotObject.transform.Find("ItemCountText").GetComponent<TextMeshProUGUI>();

        // 3. Cập nhật thông tin cho ô đồ
        if (itemIcon != null && itemData.sprite != null)
        {
            itemIcon.sprite = itemData.sprite;
            itemIcon.enabled = true; // Bật icon lên
        }

        // itemCountText.text = "x1";
        // itemCountText.gameObject.SetActive(true);
    }
}
