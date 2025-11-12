<<<<<<< HEAD
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
=======
﻿using UnityEngine;
using TMPro;
using UnityEngine.UI;
>>>>>>> d41ab707f4cdf00f098f1d3c70cdd3126e078682


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
    void Start()
    {
       if(playerStatus != null)
        {
            healthSlider.maxValue = playerStatus.maxHealth;
            healthSlider.value = playerStatus.maxHealth;
            armorSlider.maxValue = playerStatus.maxArmor;
            armorSlider.value = 0;
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
    
  
}
