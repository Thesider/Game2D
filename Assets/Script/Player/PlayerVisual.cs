using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerVisual : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Sprite defaultPlayerSprite; // Sprite mặc định

    private void Awake()
    {
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
}
