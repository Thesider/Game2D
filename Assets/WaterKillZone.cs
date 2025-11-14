using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WaterKillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {   
       
        
            PlayerStatus playerStatus = collision.GetComponent<PlayerStatus>();

            if (playerStatus != null)
            {
                
                playerStatus.ForceKill();
                Debug.Log("Player touched water -> Health = 0 (Dead)");
            }
        }
    }
}
