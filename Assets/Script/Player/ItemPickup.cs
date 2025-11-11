using NUnit.Framework.Interfaces;
using UnityEngine;

public class ItemPickup : MonoBehaviour, ICollectable
{
    [Tooltip("Kéo file Scriptable Object DataItem tương ứng")]
    [SerializeField] private ItemData dataItem;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Collect(PlayerStatus playerStatus)
    {
       if(dataItem == null)
        {
            Debug.LogWarning("Item này không có DataItem được gán!", gameObject);
            return; // Dừng hàm tại đây nếu không có data.
        }

       switch(dataItem.type)
        {
            case ItemType.Health:               
                playerStatus.Heal(dataItem.amount);
                Destroy(gameObject);
                break;
            case ItemType.Armor:
               
                playerStatus.AddArmor(dataItem.amount);
                Destroy(gameObject);
                break;
            case ItemType.SpeedBoost:
                float speedModifier = 1 + (dataItem.duration / 100f); // Giả sử point là phần trăm tăng tốc
                playerStatus.ApplySpeedModifier(dataItem.amount,dataItem.duration);
                Destroy(gameObject);
                break;
            case ItemType.Invincibility:
                
                playerStatus.ActivateInvincibility(dataItem.duration);
                Destroy(gameObject);
                break;
            case ItemType.Treasure:
                
                // Giả sử treasure chỉ tăng điểm
                playerStatus.AddScore(dataItem.point);
                Destroy(gameObject);
                break;
            case ItemType.Weapon:

                // Giả sử vũ khí chỉ tăng điểm
                if (dataItem.weaponData != null)
                {
                    // playerStats ở đây là script PlayerStats/PlayerStatus
                    // GetComponent<PlayerCombat>() sẽ tìm script PlayerCombat trên cùng GameObject Player
                    PlayerCombat combatController = playerStatus.GetComponent<PlayerCombat>();

                    if (combatController != null)
                    {
                        combatController.EquipWeapon(dataItem.weaponData);
                        Destroy(gameObject);
                    }
                }
                break;

                break;
            case ItemType.BlindBox:
                ItemBag selfItemBag = GetComponent<ItemBag>();

                if (selfItemBag != null)
                {
                    // Lấy vị trí trước khi phá hủy
                    Vector3 spawnPos = transform.position;
                    // Phá hủy blind box
                    Destroy(gameObject);
                    Debug.Log("Người chơi đã mở BlindBox");
                    // Gọi SpawnLoot từ ItemBag của chính nó
                    selfItemBag.SpawnLoot(spawnPos);
                }
                else
                {
                    Debug.LogError("BlindBox này không có component ItemBag!", gameObject);
                }
                break;
            default:
                Debug.LogWarning("Item type " + dataItem.type + " chưa được xử lý!");
                break;
        }

    }
}
