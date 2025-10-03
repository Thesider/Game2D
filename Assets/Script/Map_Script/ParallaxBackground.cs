using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
   [SerializeField] private Transform cameraTransform;
    [SerializeField] private float parallaxEffect;

    private Vector3 lastCameraPosition;

    private void Start()
    {
        lastCameraPosition = cameraTransform.position;
    }

    private void Update()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffect, deltaMovement.y * parallaxEffect, 0);
        lastCameraPosition = cameraTransform.position;
    }
}
