using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;


public class GameManager : MonoBehaviour
{

    public int score = 0;
    [SerializeField] TextMeshProUGUI scoreText;

    [Header("Player References")]
    [Tooltip("Kéo game object Player vào đây")]
    public PlayerStatus playerStatus;

    [Tooltip("Kéo GameObject Player vào đây")]
    public PlayerCombat playerCombat;

    [Header("UI Elements")]
    [Tooltip("Kéo Slider HealthBar vào đây")]
    public Slider healthSlider;
    [Tooltip("Kéo Slider ArmorBar vào đây")]
    public Slider armorSlider;
    [Tooltip("Kéo Text AmmoText vào đây")]
    public TextMeshProUGUI ammoText;
    [Tooltip("Kéo Image WeaponIcon vào đây")]
    public Image weaponIcon;
    [Tooltip("Kéo Text WeaponNameText vào đây")]
    public TextMeshProUGUI weaponNameText;
    [Tooltip("Kéo Slider InvincibilityBar vào đây")]
    public Slider invincibilitySlider;
    private Coroutine invincibilityUICoroutine;
    void Start()
    {
       if(playerStatus != null)
        {
            healthSlider.maxValue = playerStatus.maxHealth;
            healthSlider.value = playerStatus.maxHealth;
            armorSlider.maxValue = playerStatus.maxArmor;
            armorSlider.value = 0;
        }
        if (invincibilitySlider != null)
        {
            invincibilitySlider.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerStatus != null)
        {
            healthSlider.value = playerStatus.GetCurrentHealth();
            armorSlider.value = playerStatus.GetCurrentArmor();

            scoreText.text = "SCORE: " + playerStatus.GetScore().ToString("D6"); // "D6" để hiển thị 6 chữ số, ví dụ: 000500
        }
        if (playerCombat != null )
        {
            ammoText.text = "AMMO: " + playerCombat.GetCurrentAmmo();

            WeaponData currentWeapon = playerCombat.GetCurrentWeapon();
            if (currentWeapon != null)
            {
                // Nếu vũ khí có icon, hiển thị nó. Nếu không, ẩn Image đi.
                if (currentWeapon.weaponIcon != null)
                {
                    weaponIcon.sprite = currentWeapon.weaponIcon;
                    weaponIcon.enabled = true;
                }
                else
                {
                    weaponIcon.enabled = false;
                }
                // Hiển thị tên vũ khí
                weaponNameText.text = currentWeapon.weaponName;
            }
        }
    }

    public void ShowInvincibilityTimer(float duration)
    {
        // Dừng coroutine cũ nếu có để reset lại thanh đếm ngược
        if (invincibilityUICoroutine != null)
        {
            StopCoroutine(invincibilityUICoroutine);
        }
        // Bắt đầu một coroutine mới để quản lý UI
        invincibilityUICoroutine = StartCoroutine(InvincibilityTimerRoutine(duration));
    }

    private IEnumerator InvincibilityTimerRoutine(float duration)
    {
        // 1. Hiện thanh Slider lên
        invincibilitySlider.gameObject.SetActive(true);
        invincibilitySlider.maxValue = duration;
        invincibilitySlider.value = duration;

        float timer = duration;

        // 2. Vòng lặp đếm ngược
        while (timer > 0)
        {
            // Cập nhật giá trị của slider mỗi frame
            invincibilitySlider.value = timer;
            timer -= Time.deltaTime;
            yield return null; // Chờ đến frame tiếp theo
        }

        // 3. Hết giờ, ẩn thanh Slider đi
        invincibilitySlider.gameObject.SetActive(false);
        invincibilityUICoroutine = null;
    }


}
