using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WaterKillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu đối tượng va chạm có tag là "Player"
        if (collision.CompareTag("Player"))
        {   
       
            // Lấy component PlayerStatus trên player
            PlayerStatus playerStatus = collision.GetComponent<PlayerStatus>();

            if (playerStatus != null)
            {
                // Gọi hàm ForceKill để set máu về 0
                playerStatus.ForceKill();
                Debug.Log("Player touched water -> Health = 0 (Dead)");
            }
        }
    }
}
