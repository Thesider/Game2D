using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerAimAndShoot : MonoBehaviour {

    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawnPoint;

    private GameObject bulletInstance;

    private Vector2 worldPositon; 
    private Vector2 direction;
    private Vector3 originScale;
    private float angle;

    public void Awake() {
        originScale = this.gun.transform.localScale;
    }
    private void Update() {
        HandleGunRotation();
        HandleGunShooting();
    }

    private void HandleGunRotation() {
        // rotate arm
        worldPositon = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        direction = (worldPositon - (Vector2)gun.transform.position).normalized;
        gun.transform.right = direction;
        Debug.Log(angle);
        //flip arm 90deg

        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Vector3 localScale = new Vector3(this.gun.transform.localScale.x, this.gun.transform.localScale.y, 1);
        if (angle > 90 || angle < -90) localScale.y = -originScale.y;
        else localScale.y = originScale.y;
        gun.transform.localScale = localScale;
    }

    private void HandleGunShooting() {
        // spawn bullet
        if(Mouse.current.leftButton.wasPressedThisFrame) bulletInstance = Instantiate(bullet, bulletSpawnPoint.position, gun.transform.rotation);
    }
}
