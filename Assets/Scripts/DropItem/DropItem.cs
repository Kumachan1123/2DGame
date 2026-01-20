using UnityEngine;

/// <summary>
/// ドロップアイテムの取得処理
/// </summary>
public class DropItem : MonoBehaviour
{

    // 取得するタイルデータ
    public TileData tileData;

    /// <summary>
    /// プレイヤーに触れたら取得
    /// </summary>
    /// <param name="collision">接触した相手</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /// Playerタグに触れたら取得
        if (collision.CompareTag("Player"))
        {
            /// ここでインベントリに追加する予定
            Debug.Log("Item Picked");
            collision.GetComponent<PlayerInventory>().currentTile = this.tileData;

            /// 自分を削除
            Destroy(gameObject);
        }
    }
}
