using UnityEngine;
[CreateAssetMenu(fileName = "newAggressiveWeaponData", menuName = "Data/Weapon Data/Aggressive Weapon")]
public class SO_AggressiveWeaponData : SO_WeaponData {
    [SerializeField] private WeaponAttackDetail attackDetails;
    public WeaponAttackDetail AttackDetails { get => attackDetails; private set => attackDetails = value; }


}
