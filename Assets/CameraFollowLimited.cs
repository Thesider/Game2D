using UnityEngine;

public class CameraFollowLimited : MonoBehaviour
{
    public Transform target;         // Player
    public Transform mapBounds;      // Background hoặc Tilemap chính

    private float halfHeight;
    private float halfWidth;
    private float minX, maxX;

    void Start()
    {
        Camera cam = Camera.main;
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;

        SpriteRenderer sr = mapBounds.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float mapWidth = sr.bounds.size.x;
            float centerX = sr.bounds.center.x;

            minX = centerX - mapWidth / 2 + halfWidth;
            maxX = centerX + mapWidth / 2 - halfWidth;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 newPos = transform.position;
        newPos.x = Mathf.Clamp(target.position.x, minX, maxX);
        transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);
    }
}
