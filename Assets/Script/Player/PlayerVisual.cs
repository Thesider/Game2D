using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerVisual : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Sprite defaultPlayerSprite; // Sprite mặc định
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        // Lưu lại sprite ban đầu của người chơi
        defaultPlayerSprite = spriteRenderer.sprite; // Lưu sprite mặc định
    }

    // doi sprite theo vu khi
    public void ChangeToWeaponSprite(Sprite weaponSprite)
    {
        if (weaponSprite != null)
        {
            spriteRenderer.sprite = weaponSprite;
        }
    }
    //Quay lai sprite mac dinh
    public void ResetToDefaultSprite()
    {
        spriteRenderer.sprite = defaultPlayerSprite;
    }

    public void SetInvincible(bool isInvincible)
    {
        // Luôn kiểm tra xem animator có tồn tại không để tránh lỗi
        if (animator != null)
        {
           
            animator.SetBool("isInvincible", isInvincible);
        }
    }
}
