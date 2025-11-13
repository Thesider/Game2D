using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Camera và Tốc độ parallax")]
    public Transform cameraTransform;     // Camera chính
    public float parallaxMultiplier = 0.5f; // Tốc độ di chuyển tương đối
    private float textureUnitSizeX;        // Chiều rộng texture theo world unit
    private Vector3 lastCameraPosition;    // Vị trí camera trước đó

    private void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        lastCameraPosition = cameraTransform.position;

        // Tính chiều rộng của background (tính theo world unit)
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = (texture.width / sprite.pixelsPerUnit) * transform.localScale.x;
    }

    private void Update()
    {
        // Tính độ dịch chuyển của camera
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxMultiplier, 0f, 0f);
        lastCameraPosition = cameraTransform.position;

        // Nếu camera đi xa hơn chiều rộng của texture -> lặp lại background
        if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
        {
            float offset = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(cameraTransform.position.x + offset, transform.position.y, transform.position.z);
        }
    }
}
