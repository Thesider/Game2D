using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Vũ khí mặc định của người chơi, ví dụ: súng lục")]
    [SerializeField] private WeaponData defaultWeapon;

    private WeaponData currentWeapon;
    private int currentAmmo;
    private Coroutine weaponCoroutine;
    private PlayerVisual playerVisuals;
   

    void Start()
    {
        // Bắt đầu game với vũ khí mặc định
        EquipWeapon(defaultWeapon);
    }

    private void Awake()
    {
        playerVisuals = GetComponent<PlayerVisual>();
    }

    public void EquipWeapon(WeaponData newWeaponData)
    {
        if (weaponCoroutine != null)
        {
            StopCoroutine(weaponCoroutine);
            weaponCoroutine = null;
        }

        if (playerVisuals != null)
        {
            if (newWeaponData == defaultWeapon || newWeaponData.playerSpriteWithWeapon == null)
            {
                // Nếu trang bị vũ khí mặc định hoặc vũ khí mới không có sprite riêng
                playerVisuals.ResetToDefaultSprite();
            }
            else
            {
                // Nếu trang bị vũ khí mới có sprite riêng
                playerVisuals.ChangeToWeaponSprite(newWeaponData.playerSpriteWithWeapon);
            }
        }
        currentWeapon = newWeaponData;

        if (newWeaponData.usageDuration > 0)
        {
            currentAmmo = 999;
            weaponCoroutine = StartCoroutine(WeaponTimer(newWeaponData.usageDuration));
            Debug.Log("Trang bị vũ khí: " + currentWeapon.weaponName + " (Thời gian: " + newWeaponData.usageDuration + "s)");
        }
        else
        {
            currentAmmo = newWeaponData.ammoAmount;
            Debug.Log("Trang bị vũ khí: " + currentWeapon.weaponName + " (Đạn: " + currentAmmo + ")");
        }
    }

    private IEnumerator WeaponTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        Debug.Log("Hết giờ! Quay về vũ khí mặc định.");
        EquipWeapon(defaultWeapon);
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.KeypadEnter)) // Ví dụ: Bắn bằng phím Space
    //    {
    //        Shoot();
    //    }
    //}

    public void Shoot()
    {
        if (currentWeapon != null && currentAmmo > 0)
        {
            if (currentWeapon.usageDuration <= 0)
            {
                currentAmmo--;
            }

            BulletPool.Spawn(
                currentWeapon.bulletPrefab,
                transform.position,
                Quaternion.identity,
                transform.right,
                currentWeapon.bulletSpeed,
                currentWeapon.bulletDamage,
                currentWeapon.bulletLifetime,
                gameObject
            );

            if (currentAmmo <= 0 && currentWeapon.usageDuration <= 0)
            {
                Debug.Log("Hết đạn! Quay về vũ khí mặc định.");
                EquipWeapon(defaultWeapon);
            }
        }
    }

    public string GetCurrentAmmo()
    {
        // Trả về một chuỗi string đã được định dạng sẵn
        if (currentWeapon != null && currentWeapon.usageDuration > 0)
        {
            // Nếu là vũ khí có thời hạn, hiển thị "vô cực"
            return "INF";
        }
        return currentAmmo.ToString();
    }

    public WeaponData GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
