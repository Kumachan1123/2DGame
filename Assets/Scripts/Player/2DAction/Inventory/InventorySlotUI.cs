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
