using UnityEngine;

public class ParallaxRepeater : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxMultiplier = 0.5f;

    private float startPosX;
    private float length;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        startPosX = transform.position.x;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        length = sr.bounds.size.x;
    }

    void LateUpdate()
    {
        float temp = cameraTransform.position.x * (1 - parallaxMultiplier);
        float dist = cameraTransform.position.x * parallaxMultiplier;

        transform.position = new Vector3(startPosX + dist, transform.position.y, transform.position.z);

        // Nếu camera di chuyển ra ngoài chiều dài background, dịch sang để lặp lại
        if (temp > startPosX + length)
            startPosX += length;
        else if (temp < startPosX - length)
            startPosX -= length;
    }
}
