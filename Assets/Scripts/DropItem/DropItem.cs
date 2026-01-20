using UnityEngine;

/// <summary>
/// ドロップアイテムの取得処理
/// </summary>
public class DropItem : MonoBehaviour
{

    // 取得するタイルデータ
    public TileData tileData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.GetComponent<PlayerInventory>();
            InventoryUI ui = collision.GetComponentInChildren<InventoryUI>();

            if (inventory != null && tileData != null)
            {
                bool added = inventory.AddItem(tileData, 1); // Inventory に追加
                if (added)
                {
                    Destroy(gameObject); // アイテム消す
                    if (ui != null)
                        ui.RefreshAll(); // UI 更新
                }
            }
        }
    }




}
