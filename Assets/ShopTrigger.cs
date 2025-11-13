using UnityEngine;
using TMPro; // N?u b?n mu?n dùng TextMeshPro cho thông báo

public class ShopTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    [Tooltip("M?t ??i t??ng hình ?nh/text (ví d?: ch? 'E' ho?c 'Talk') ?? báo cho ng??i ch?i bi?t có th? t??ng tác. S? hi?n ra khi ng??i ch?i ? g?n.")]
    public GameObject interactionCue;

    [Header("Shop Reference")]
    [Tooltip("Kéo GameObject ShopListingPanel (ch?a script ShopManager) vào ?ây")]
    public ShopManager shopManager;

    private bool playerIsNearby = false;
    private IShopCustomer customer;

    void Awake()
    {
        // ?n g?i ý t??ng tác ?i lúc ban ??u
        if (interactionCue != null)
        {
            interactionCue.SetActive(false);
        }
    }

    void Update()
    {// Log 1: Để xem biến playerIsNearby có đúng là true không.
        if (playerIsNearby)
        {
            Debug.Log("Player đang ở gần. Đang chờ nhấn phím E...");
        }

        // Log 2: Để xem Unity có nhận diện được phím E đang được nhấn không.
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Phím E đã được nhấn!");
        }

        // Kiểm tra kết hợp cả hai điều kiện
        if (playerIsNearby && Input.GetKeyDown(KeyCode.E))
        {
            // Log 3: Dòng này phải được in ra nếu mọi thứ đều đúng.
            Debug.Log("Tất cả điều kiện thỏa mãn! Đang cố gắng mở shop...");

            if (shopManager != null && customer != null)
            {
                shopManager.OpenShop(customer);
            }
            else
            {
                Debug.LogError("LỖI: shopManager hoặc customer đang bị null!");
            }
        }
    }

    // Hàm này ???c Unity g?i khi có m?t ??i t??ng khác ?i VÀO vùng Trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Log 1: Để xem hàm có được gọi không
        Debug.Log("OnTriggerEnter2D được gọi bởi: " + other.name);
        // Ki?m tra xem ??i t??ng ?ó có ph?i là Player không
        if (other.CompareTag("Player"))
        {
            // Log 2: Để xem có nhận đúng Player không
            Debug.Log("Đối tượng là Player!");

            // L?y v? "danh tính khách hàng" t? Player
            customer = other.GetComponent<IShopCustomer>();

            // N?u Player ?úng là m?t khách hàng h?p l?
            if (customer != null)
            {
                // Log 3: Để xem có lấy được component khách hàng không
               
                playerIsNearby = true;
                // Hi?n g?i ý t??ng tác lên
                if (interactionCue != null)
                {
                    Debug.Log("Tìm thấy IShopCustomer. Chuẩn bị hiện Cue.");
                    interactionCue.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("InteractionCue chưa được gán trong Inspector!");
                }
            }
        }
    }

    // Hàm này ???c Unity g?i khi ??i t??ng ?ó ?i RA KH?I vùng Trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = false;
            customer = null;
            // ?n g?i ý t??ng tác ?i
            if (interactionCue != null)
            {
                interactionCue.SetActive(false);
            }
        }
    }
}