using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData : ScriptableObject {
    public int currentHealth;
    public int currentArmor;
    public int currentLives;
    public Vector3 deathPosition;
    public Vector3 savePosition; // the custom position you want to save

    public PlayerData() { }

    public PlayerData(PlayerStatus player)
    {
        currentHealth = player.GetCurrentHealth();
        currentArmor = player.GetCurrentArmor();

        // You'll need to make currentLives and deathPosition accessible:
        currentLives = player.CurrentLives;
        deathPosition = player.DeathPosition;
        savePosition = player.transform.position;
    }

    [Header("Move State")]
    public float moveSpeed = 10f;

    [Header("Jump State")]
    public float jumpVelocity = 15f;
    public int amountOfJumps = 1;

    [Header("In Air State")]
    public float coyoteTime = 0.2f;
    public float variableJumpHeightMultiplier = 0.5f;

    [Header("Dash State")]
    public float dashCooldown = 1f;
    public float mmaxHoldTime = 0.75f;
    public float dashTime = 0.2f;
    public float dashVelocity = 20f;
    public float drag = 10f; 
    public float distanceBetweenAfterImages = 0.5f;

    [Header("Crouch States")]
    public float crouchSpeed = 3f;
    public float crouchColliderHeight = 0.172f;
    public float standColliderHeight = 0.343f;

}
