using UnityEngine;

public interface IShopCustomer
{
    // Hàm để kiểm tra xem khách hàng có đủ tiền không
    bool CanAfford(int amount);

    // Hàm để tiêu tiền
    void SpendGold(int amount);

    // Hàm được gọi sau khi mua hàng thành công để nhận vật phẩm
    void BoughtItem(ItemData itemData);

    int GetGoldAmount();

}
