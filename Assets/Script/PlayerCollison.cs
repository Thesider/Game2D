using UnityEngine;


public class PlayerCollison : MonoBehaviour
{
    //[SerializeField] private GameManager gameManager;
    //public ItemBag itemBag;
    //public Sprite newSprite;
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Fish"))
    //    {
    //        gameManager.addScore(10);
    //        Destroy(collision.gameObject);
    //    }
    //    else if (collision.CompareTag("H_letter"))
    //    {
    //        gameManager.addScore(50);

    //        var sr = gameObject.GetComponent<SpriteRenderer>();
    //        if (sr != null)
    //        {
    //            // Đổi sang sprite mới (kéo sprite này từ Inspector)
    //            sr.sprite = newSprite;

    //        }
    //        Destroy(collision.gameObject);

    //    }
    //    else if (collision.CompareTag("Treasure"))
    //    {
    //        gameManager.addScore(100);
    //        Destroy(collision.gameObject);
    //    }
    //    else if (collision.CompareTag("BlindBox"))
    //    {
    //        Destroy(collision.gameObject);
    //        itemBag.SpawnLoot(collision.transform.position);

    //    }
    //}

    private PlayerStatus playerStatus;

    private void Awake()
    {
        // Lấy script PlayerStats của chính mình
        playerStatus = GetComponent<PlayerStatus>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       ICollectable collectable = collision.GetComponent<ICollectable>();

        if (collectable != null)
        {
            collectable.Collect(playerStatus);
        }
    }

}
