using System.Collections; // Cần thêm dòng này để dùng Coroutine
using UnityEngine;

public class SuicideBot : MonoBehaviour
{
    [Header("Thiết lập Drone")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float explosionRange = 1f;
    [SerializeField] private float damage = 25f;

    [Header("Kích hoạt (Activation)")]
    [Tooltip("Thời gian chờ trước khi đuổi theo Player (giây)")]
    [SerializeField] private float chaseDelay = 1.5f; // Sẽ đứng im 1.5 giây

    [Header("Hiệu ứng")]
    [SerializeField] private GameObject explosionEffectPrefab;

    private Transform playerTarget;
    private bool isActivated = false; // Biến cờ: = false (đang chờ), = true (đang đuổi)

    void Start()
    {
        // 1. Bắt đầu đếm ngược thời gian chờ (delay)
        // Nó sẽ KHÔNG đuổi theo Player ngay lập tức
        StartCoroutine(ActivateChase());
    }

    void Update()
    {
        // 2. Nếu drone chưa được kích hoạt (isActivated == false)
        // hoặc không tìm thấy Player (playerTarget == null)
        // thì KHÔNG làm gì cả (đứng im lơ lửng)
        if (!isActivated || playerTarget == null)
        {
            return; 
        }

        // --- 3. Logic đuổi theo (CHỈ chạy khi isActivated == true) ---
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= explosionRange)
        {
            Explode();
        }
        else
        {
            MoveTowardsPlayer();
        }
    }

    // Coroutine: Kích hoạt việc đuổi theo
    private IEnumerator ActivateChase()
    {
        // 4. Chờ (đứng im lơ lửng) trong 'chaseDelay' giây
        yield return new WaitForSeconds(chaseDelay);

        // 5. Sau khi chờ xong, BẮT ĐẦU tìm Player
        Debug.Log("Drone " + gameObject.name + " BẮT ĐẦU ĐUỔI THEO!");
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            playerTarget = playerObject.transform;
            isActivated = true; // "Bật" drone lên, cho phép nó đuổi theo ở hàm Update()
        }
        else
        {
            // Nếu không tìm thấy Player, tự hủy
            
            Debug.LogWarning("Drone không tìm thấy Player, tự hủy.");
            Destroy(gameObject);
        }
    }

    // Hàm di chuyển (giữ nguyên)
    private void MoveTowardsPlayer()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            playerTarget.position,
            moveSpeed * Time.deltaTime
        );
        
    }
    

    private void Explode()
    {
        Debug.Log("BOOM! Drone tự hủy.");
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Gây sát thương cho Player (nếu tìm thấy)
        if (playerTarget != null && playerTarget.TryGetComponent(out PlayerStatus playerStatus))
        {
            // Sửa (int)damage nếu hàm TakeDamage của bạn nhận int
            playerStatus.TakeDamage((int)damage); 
        }
        Destroy(gameObject);
    }
}