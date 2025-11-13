using System.Collections;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    // --- CÁC BIẾN (PROPERTIES) ---
    [Header("--- SỨC KHỎE (HEALTH) ---")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("--- GIÁP (ARMOR) ---")]
    public int maxArmor = 50;
    private int currentArmor;

    [Header("--- DI CHUYỂN (MOVEMENT) ---")]
    public float baseMoveSpeed = 5f;
    //[HideInInspector] 
    public float currentMoveSpeed;

    [Header("--- TRẠNG THÁI & BUFFS ---")]
    public float speedModifier = 1.0f;
    public bool isInvincible = false;
    [Tooltip("Thời gian bất tử sau khi hồi sinh")]
    public float respawnInvincibilityDuration = 2f; // MỚI: Thời gian bất tử sau khi hồi sinh

    [Header("--- HỒI SINH (LIVES) ---")]
    public int maxLives = 1;
    private int currentLives;
    private Vector3 deathPosition; // Biến lưu vị trí

    // MỚI: Biến để theo dõi coroutine đang chạy
    private Coroutine invincibilityCoroutine;
    private Coroutine speedModifierCoroutine;
    // ... (Hàm Awake và Update giữ nguyên) ...

    //biến để lưu điểm
    private int score = 0;

    public int CurrentLives => currentLives;
    public Vector3 DeathPosition => deathPosition;


    [SerializeField] private HealthBarSlider HealthBarSlider;
    void Awake()
    {
        currentHealth = maxHealth;
        currentArmor = 0;
        currentLives = maxLives;
        UpdateSpeed();

        if (HealthBarSlider == null)
        {
            HealthBarSlider = GetComponentInChildren<HealthBarSlider>();
            if (HealthBarSlider == null)
            {
                Debug.LogWarning("PlayerStatus: No HealthBarSlider found. Health UI will not update until assigned.");
            }
        }

        if (HealthBarSlider != null)
        {
            HealthBarSlider.SetHealth(currentHealth);
        }
    }

    void Update()
    {
        UpdateSpeed();
    }

    private void UpdateSpeed()
    {
        currentMoveSpeed = baseMoveSpeed * speedModifier;
    }

    // --- CÁC HÀM CÔNG KHAI (PUBLIC METHODS) ---


    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            Debug.Log("Player is INVINCIBLE, no damage taken.");
            return;
        }

        int remainingDamage = damage;

        // 2. Trừ vào giáp trước (lớp bảo vệ ngoài)
        if (currentArmor > 0)
        {
            if (currentArmor >= remainingDamage)
            {
                // Nếu giáp đủ để chặn hết sát thương
                currentArmor -= remainingDamage;
                remainingDamage = 0; // Không còn sát thương để trừ vào máu
                Debug.Log("Armor absorbed all damage. Armor left: " + currentArmor);
            }
            else
            {
                // Nếu giáp không đủ, trừ hết giáp và tính sát thương còn lại
                remainingDamage -= currentArmor;
                currentArmor = 0;
                Debug.Log("Armor broke! Remaining damage: " + remainingDamage);
            }
        }

        // 3. Trừ sát thương còn lại vào máu (lõi HP)
        if (remainingDamage > 0)
        {
            currentHealth -= remainingDamage;
            if (HealthBarSlider != null)
            {
                HealthBarSlider.SetHealth(currentHealth);
            }
            else
            {
                Debug.LogWarning("PlayerStatus: HealthBarSlider is not assigned. Damage applied without UI update.");
            }
            Debug.Log("Health took " + remainingDamage + " damage. Health left: " + currentHealth);
        }

        // 4. Kiểm tra xem người chơi đã chết chưa
        if (currentHealth <= 0)
        {
            currentHealth = 0; // Đảm bảo máu không bị âm
            Die();
        }
    }
    // Hàm hồi máu
    public void Heal(int amount)
    {
        currentHealth += amount;
        // Đảm bảo máu không vượt quá tối đa
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log("Player healed " + amount + " HP. Health: " + currentHealth + "/" + maxHealth);
    }

    // Hàm thêm giáp
    public void AddArmor(int amount)
    {
        currentArmor += amount;
        if (currentArmor > maxArmor)
        {
            currentArmor = maxArmor;
        }
        Debug.Log("Player gained " + amount + " armor. Armor: " + currentArmor + "/" + maxArmor);
    }

    // Hàm thay đổi hệ số tốc độ
    // --- HÀM MỚI ĐỂ ÁP DỤNG HIỆU ỨNG TỐC ĐỘ ---
    public void ApplySpeedModifier(float modifier, float duration)
    {
        // Nếu đang có một hiệu ứng tốc độ khác, hủy nó đi để hiệu ứng mới được áp dụng.
        if (speedModifierCoroutine != null)
        {
            StopCoroutine(speedModifierCoroutine);
        }

        // Bắt đầu một Coroutine mới để quản lý hiệu ứng này.
        speedModifierCoroutine = StartCoroutine(SpeedModifierRoutine(modifier, duration));
    }

    // --- COROUTINE MỚI ĐỂ ĐẾM NGƯỢC THỜI GIAN HIỆU LỰC ---
    private IEnumerator SpeedModifierRoutine(float modifier, float duration)
    {
        Debug.Log("Tốc độ thay đổi! Hệ số mới: " + modifier + " trong " + duration + " giây.");

        // 1. Áp dụng hệ số tốc độ mới
        speedModifier = modifier;

        // 2. Chờ hết thời gian hiệu lực
        yield return new WaitForSeconds(duration);

        // 3. Sau khi chờ xong, trả tốc độ về bình thường
        Debug.Log("Hiệu ứng tốc độ đã hết. Trở về bình thường.");
        speedModifier = 1.0f;

        // Reset biến coroutine để báo rằng không còn hiệu ứng nào đang chạy
        speedModifierCoroutine = null;
    }

    // MỚI: Hàm công khai để kích hoạt trạng thái bất tử
    public void ActivateInvincibility(float duration)
    {
        // Nếu đang có một coroutine bất tử chạy, hãy dừng nó lại
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
        }

        // Bắt đầu một coroutine mới và lưu lại tham chiếu của nó
        invincibilityCoroutine = StartCoroutine(InvincibilityRoutine(duration));
    }

    // MỚI: Coroutine xử lý việc bất tử theo thời gian
    private IEnumerator InvincibilityRoutine(float duration)
    {
        Debug.Log("Player became INVINCIBLE for " + duration + " seconds.");
        isInvincible = true;

        // TODO: Thêm hiệu ứng nhấp nháy cho player ở đây để người chơi biết đang bất tử

        // Tạm dừng hàm này trong 'duration' giây
        yield return new WaitForSeconds(duration);

        // Sau khi chờ xong, thực hiện phần code bên dưới
        isInvincible = false;
        // TODO: Tắt hiệu ứng nhấp nháy
        Debug.Log("Player is no longer invincible.");

        invincibilityCoroutine = null; // Reset biến coroutine
    }

    // --- CÁC HÀM XỬ LÝ SỰ KIỆN (EVENT HANDLERS) ---

    private void Die()
    {
        Debug.Log("💀 Player has DIED!");
        currentLives--;

        deathPosition = transform.position; // Save death position

        if (currentLives > 0)
        {
            // Respawn after 3 seconds
            Invoke(nameof(Respawn), 3f);
        }
        else
        {
            Debug.Log("GAME OVER");
            gameObject.SetActive(false);

            // ✅ Show Death Screen
            DeadScreen deadScreen = FindObjectOfType<DeadScreen>();
            if (deadScreen != null)
            {
                Time.timeScale = 0f; // Pause the game
                deadScreen.Show();
            }
            else
            {
                Debug.LogWarning("No DeadScreen found in the scene!");
            }
        }
    }

    public void Respawn()
    {

        Debug.Log("Player is RESPAWNING!");
        // Hiện lại sprite và bật lại va chạm
        gameObject.SetActive(true);
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        currentHealth = maxHealth;
        currentArmor = 0;
        speedModifier = 1.0f;

        transform.position = deathPosition;

        // CẬP NHẬT: Kích hoạt trạng thái bất tử tạm thời sau khi hồi sinh
        ActivateInvincibility(respawnInvincibilityDuration);
    }

    public void AddScore(int point)
    {
        score += point;
        Debug.Log("Score increased by " + point + ". Total score: " + score);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetCurrentArmor()
    {
        return currentArmor;
    }
    public int GetScore()
    {
        return score;
    }

    public void ForceKill()
    {
        if (currentHealth <= 0)
        {
            return;
        }

        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
            invincibilityCoroutine = null;
        }

        isInvincible = false;
        currentArmor = 0;
        currentHealth = 0;
        if (HealthBarSlider != null)
        {
            HealthBarSlider.SetHealth(currentHealth);
        }

        Die();
    }

    public void SavePlayerData()
    {
        PlayerData data = new PlayerData(this);
        SaveSystem.Save(data);
        Debug.Log("✅ Player data saved!");
    }

    public void LoadPlayerData()
    {
        PlayerData data = new PlayerData();
        SaveSystem.Load(data);

        currentHealth = data.currentHealth;
        currentArmor = data.currentArmor;
        currentLives = data.currentLives;
        deathPosition = data.deathPosition;

        // Move player to saved position
        transform.position = data.savePosition;

        if (HealthBarSlider != null)
            HealthBarSlider.SetHealth(currentHealth);

        Debug.Log($"✅ Player loaded. HP: {currentHealth}, Armor: {currentArmor}, Lives: {currentLives}");
    }

}
