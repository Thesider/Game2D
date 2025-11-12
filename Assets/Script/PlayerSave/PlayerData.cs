using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
[Serializable]
public class PlayerData : ScriptableObject
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int coins;
}
