using System.Collections.Generic;
using UnityEngine;

public class ItemBag : MonoBehaviour
{
    public GameObject droppedItemPrefab;
    public List<ItemData> dataItemList = new List<ItemData>();


    public ItemData GetDroppedItem()
    {
        // int randomNumber = Random.Range(1, 100);
        List<ItemData> possibleItems = new List<ItemData>();
        foreach (ItemData item in dataItemList)
        {
            if (0 <= item.dropChance)
            {
                possibleItems.Add(item);

            }
        }
        if (possibleItems.Count > 0)
        {
            ItemData droppedItem = possibleItems[Random.Range(0, possibleItems.Count)];
            return droppedItem;
        }

        return dataItemList[0];
    }

    //public void SpawnLoot(Vector3 spawnPosition)
    //{
    //    ItemData droppedItem = GetDroppedItem();

    //    if(droppedItem!= null)
    //    {
    //        GameObject itemGameObject = Instantiate(droppedItemPrefab, spawnPosition, Quaternion.identity);
    //        itemGameObject.GetComponent<SpriteRenderer>().sprite = droppedItem.sprite;
    //    }
    //}
    public void SpawnLoot(Vector3 spawnPosition)
    {
        Debug.Log("SpawnLoot called at position: " + spawnPosition);

        ItemData droppedItem = GetDroppedItem();
        if (droppedItem == null)
        {
            Debug.Log("Dropped item is NULL");
            return;
        }
        //Tạo (instantiate) một bản sao của prefab droppedItemPrefab tại vị trí spawn được truyền vào
        GameObject go = Instantiate(droppedItemPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("Instantiated prefab: " + go.name);
        //Lấy component SpriteRenderer của prefab vừa tạo, vì cần thay đổi hình ảnh sprite để hiển thị đúng vật phẩm.
        var sr = go.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.Log("Prefab has no SpriteRenderer!");
        }
        else
        {
            sr.sprite = droppedItem.sprite;
            Debug.Log("Set sprite to: " + droppedItem.sprite);

        }
        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 1.5f;        // đảm bảo có trọng lực

            float dropForce = 4.5f;        // lực ban đầu
                                           // Vector2(x, y): x > 0 sang phải, y > 0 nhích lên
            Vector2 dropDirection = new Vector2(Random.Range(0.8f, 1.2f), Random.Range(0.3f, 0.8f)).normalized;

            rb.AddForce(dropDirection * dropForce, ForceMode2D.Impulse);
        }
    }

}
