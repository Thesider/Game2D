using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class ShopManager : MonoBehaviour
{
    [Header("Shop Configuration")]
    [Tooltip("Danh sách các vật phẩm (Scriptable Object) sẽ được bán trong shop này")]
    public List<ItemData> itemsToSell;

    [Header("UI References")]
    [Tooltip("Kéo Prefab ShopItemTemplate vào đây")]
    public GameObject shopItemPrefab;
    [Tooltip("Kéo GameObject ItemContainer (trong ScrollView) vào đây")]
    public Transform itemContainer;

    [Header("Feedback UI")]
    public TextMeshProUGUI feedbackText;
    public float feedbackDuration = 1.5f; // Thời gian hiển thị thông báo
    private Coroutine feedbackCoroutine;

    [Header("Sound Effects")]
    public AudioClip successSound;
    public AudioClip failSound;
    private AudioSource audioSource;

    public TextMeshProUGUI goldText;

    // Biến để lưu trữ tham chiếu đến khách hàng (Player)
    private IShopCustomer currentCustomer;

    void Awake() 
    {
        // Tự động thêm AudioSource nếu chưa có
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update() 
    {
        // Luôn cập nhật số vàng khi shop đang mở
        if (gameObject.transform.parent.gameObject.activeSelf && currentCustomer != null)
        {
            // Cần thêm hàm GetGold() vào interface và PlayerStats
            goldText.text = "Coins: " + (currentCustomer as PlayerStatus).GetGoldAmount();
        }
    }

    // Hàm này sẽ được gọi bởi ShopTrigger để bắt đầu phiên mua sắm
    public void OpenShop(IShopCustomer customer)
    {
        this.currentCustomer = customer;
        gameObject.transform.parent.gameObject.SetActive(true); // Bật ShopPanel lên
        PopulateShop();
    }

    // Hàm này sẽ được gọi bởi nút Close
    public void CloseShop()
    {
        gameObject.transform.parent.gameObject.SetActive(false); // Tắt ShopPanel đi
    }

    // Hàm để điền các vật phẩm vào shop
    private void PopulateShop()
    {
        Debug.Log("Bắt đầu điền item vào shop...");
        // Xóa các item cũ đi để tránh trùng lặp nếu mở shop nhiều lần
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }

        // Tạo các ô item mới từ danh sách itemsToSell
        foreach (ItemData item in itemsToSell)
        
        {
            GameObject itemObject = Instantiate(shopItemPrefab, itemContainer);

            // Tìm các component con
            Image itemIcon = itemObject.transform.Find("ItemIcon").GetComponent<Image>();
            TextMeshProUGUI itemNameText = itemObject.transform.Find("ItemNameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI itemPriceText = itemObject.transform.Find("ItemPriceText").GetComponent<TextMeshProUGUI>();
            Button buyButton = itemObject.transform.Find("BuyButton").GetComponent<Button>();

            // Gán dữ liệu vào UI
           if(item.type != ItemType.Weapon)
            {
                itemIcon.sprite = item.sprite;
                itemNameText.text = item.itemName;
                itemPriceText.text = item.price.ToString() + " G";
            }
            else
            {
                itemIcon.sprite = item.weaponData.weaponIcon;
                itemNameText.text = item.weaponData.weaponName;
                itemPriceText.text = item.price.ToString() + " G";
            }
            
            // Gán sự kiện cho nút "Buy"
            buyButton.onClick.AddListener(() => { TryBuyItem(item); });

        }
        Debug.Log("Điền item vào shop hoàn tất!");
    }

    private void TryBuyItem(ItemData itemToBuy)
    {
        if (currentCustomer == null) return;

        // 1. Kiểm tra xem khách hàng có đủ tiền không
        if (currentCustomer.CanAfford(itemToBuy.price))
        {
            audioSource.PlayOneShot(successSound);
            // 2. Nếu đủ, trừ tiền
            currentCustomer.SpendGold(itemToBuy.price);

            // 3. Trao vật phẩm cho khách hàng
            currentCustomer.BoughtItem(itemToBuy);

            // 4. (MỚI) Cập nhật túi đồ (Inventory) trên UI
            // Tìm InventoryManager và yêu cầu nó thêm item
            Transform shopPanel = transform.parent;
            InventoryManager inventoryManager = shopPanel.GetComponentInChildren<InventoryManager>();
            if (inventoryManager != null)
            {
                inventoryManager.AddItemToInventory(itemToBuy);
            }

            Debug.Log("Mua thành công: " + itemToBuy.itemName);

        }
        else
        {
            audioSource.PlayOneShot(failSound);
            //  Hiển thị thông báo "Không đủ tiền"
            ShowFeedback("Not enough gold!", Color.red);
        }

    }

    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;

        if (feedbackCoroutine != null)
        {
            StopCoroutine(feedbackCoroutine);
        }
        feedbackCoroutine = StartCoroutine(FeedbackRoutine(message, color));
    }

    private IEnumerator FeedbackRoutine(string message, Color color)
    {
        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);

        yield return new WaitForSeconds(feedbackDuration);

        feedbackText.gameObject.SetActive(false);
        feedbackCoroutine = null;
    }
}