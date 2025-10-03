using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    // Camera sẽ được tham chiếu trong Inspector (nơi lấy vị trí camera để tính hiệu ứng parallax)
    [SerializeField] private Transform cameraTransform;

    // Hệ số hiệu ứng parallax (0 = không di chuyển, 1 = di chuyển cùng tốc độ camera)
    [SerializeField] private float parallaxEffect;

    // Lưu lại vị trí trước đó của camera để so sánh khi camera di chuyển
    private Vector3 lastCameraPosition;

    private void Start()
    {
        // Khởi tạo vị trí ban đầu của camera
        lastCameraPosition = cameraTransform.position;
    }

    private void Update()
    {
        // Tính toán xem camera đã di chuyển bao nhiêu từ frame trước đến frame hiện tại
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // Di chuyển background theo một phần deltaMovement (tùy vào parallaxEffect)
        // Nếu parallaxEffect < 1 thì background sẽ di chuyển chậm hơn camera → tạo chiều sâu
        transform.position += new Vector3(deltaMovement.x * parallaxEffect, deltaMovement.y * parallaxEffect, 0);

        // Cập nhật lại vị trí camera để chuẩn bị cho frame tiếp theo
        lastCameraPosition = cameraTransform.position;
    }
}
