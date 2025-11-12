using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Player Visuals")]
    [Tooltip("Sprite của người chơi khi cầm vũ khí này (ví dụ: sprite cầm súng máy)")]
    public Sprite playerSpriteWithWeapon;

    [Header("Info")]
    [Tooltip("Tên vũ khí, có thể dùng để hiển thị trên UI")]
    public string weaponName;
    [Tooltip("Icon của vũ khí, hiển thị trên UI")]
    public Sprite weaponIcon;

    [Header("Shooting Logic")]
    [Tooltip("Số viên đạn bắn ra mỗi giây. Ví dụ: 10 = súng máy, 2 = súng lục")]
    public float fireRate;

    // --- PHẦN MỚI: TẤT CẢ THÔNG SỐ CỦA VIÊN ĐẠN ---
    [Header("Bullet Properties")]
    [Tooltip("Prefab của viên đạn mà vũ khí này bắn ra")]
    public GameObject bulletPrefab;

    [Tooltip("Tốc độ bay của viên đạn")]
    public float bulletSpeed = 15f;

    [Tooltip("Sát thương gây ra bởi mỗi viên đạn")]
    public float bulletDamage = 10f;

    [Tooltip("Viên đạn sẽ tự biến mất sau bao nhiêu giây nếu không va chạm")]
    public float bulletLifetime = 4f;
    // --- KẾT THÚC PHẦN MỚI ---

    [Header("Usage Limit")]
    [Tooltip("Lượng đạn nhận được khi nhặt vũ khí này. Đặt là 0 nếu vũ khí dùng thời gian.")]
    public int ammoAmount;

    [Tooltip("Thời gian sử dụng vũ khí (giây). Đặt là 0 nếu vũ khí dùng đạn.")]
    public float usageDuration;

    [Header("Effects")]
    [Tooltip("Âm thanh phát ra khi bắn")]
    public AudioClip shootSound;
}
