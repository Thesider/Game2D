using System.Collections;
using UnityEngine;

public class DroneSpawner : MonoBehaviour
{
    [Header("Thiết lập Prefab")]
    [SerializeField] private GameObject botPrefab; // Kéo Prefab SuicideBot vào đây

    [Header("Thiết lập Spawn")]
    [Tooltip("Số lượng drone tối đa muốn spawn")]
    [SerializeField] private int spawnCount = 10; // Tổng số Drone sẽ sinh ra

    [Tooltip("Thời gian chờ giữa mỗi lần spawn (giây)")]
    [SerializeField] private float spawnInterval = 5f; // Cứ 5 giây sinh ra 1 con

    [Tooltip("Kích thước của khu vực spawn (ngẫu nhiên)")]
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(20f, 10f); // Rộng 20, Cao 10

    // Bắt đầu khi game chạy
    void Start()
    {
        if (botPrefab == null)
        {
            Debug.LogError("Chưa gán Prefab cho DroneSpawner!");
            return;
        }

        // Bắt đầu vòng lặp tự sinh
        StartCoroutine(SpawnLoop());
    }

    // Coroutine: Vòng lặp để sinh ra Drone theo thời gian
    private IEnumerator SpawnLoop()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            // 1. Chờ theo thời gian đã định (spawnInterval)
            yield return new WaitForSeconds(spawnInterval);

            // 2. Sau khi chờ, sinh ra 1 con Drone
            SpawnBot();
        }
        
        // Vòng lặp kết thúc sau khi sinh đủ số lượng
        Debug.Log("Đã spawn đủ " + spawnCount + " drones.");
    }

    // Hàm sinh ra 1 Drone tại vị trí ngẫu nhiên
    private void SpawnBot()
    {
        // 3. Lấy vị trí ngẫu nhiên "rải rác"
        Vector3 randomPosition = GetRandomSpawnPosition();

        // 4. Tạo (Instantiate) 1 bản sao của Prefab tại vị trí đó
        Instantiate(botPrefab, randomPosition, Quaternion.identity);
    }

    // Hàm tính toán vị trí ngẫu nhiên trong khu vực
    private Vector3 GetRandomSpawnPosition()
    {
        // Vị trí trung tâm của Spawner
        Vector3 center = transform.position;

        // Tính toán ngẫu nhiên trong phạm vi
        float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomY = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);

        return new Vector3(center.x + randomX, center.y + randomY, 0);
    }

    // (Tùy chọn) Vẽ ra khu vực spawn trong Editor để dễ chỉnh sửa
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f); // Màu xanh lá mờ
        Gizmos.DrawCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0.1f));
    }
}