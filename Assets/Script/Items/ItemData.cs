using UnityEngine;
using UnityEngine.InputSystem.Controls;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public Sprite sprite;
    public int point;
    public int dropChance;
}
