using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon; // アイテムアイコン
    public InventorySlot slot;

    /// <summary>
    /// スロットを更新してアイコン表示
    /// </summary>
    public void Refresh()
    {
        // デバッグログを追加
        // スロットの有無
        Debug.Log($"InventorySlotUI: Refresh called for slot with item {(slot != null && slot.tileData != null ? slot.tileData.name : "null")}");
        // タイルデータの有無
        Debug.Log($"InventorySlotUI: Slot tileData is {(slot != null ? (slot.tileData != null ? "not null" : "null") : "slot is null")}");
        // アイコンの有無
        Debug.Log($"InventorySlotUI: Icon is {(icon != null ? "not null" : "null")}");
        if (slot != null && slot.tileData != null && icon != null)
        {
            icon.sprite = slot.tileData.Icon;
            icon.enabled = true;
        }
        else if (icon != null)
        {
            icon.enabled = false;
        }
    }


}
