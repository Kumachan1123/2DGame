using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    // アイテムアイコン
    public Image icon;
    // フレーム
    public Image frame;
    // 選択時のフレームの色
    public Color selectedFrameColor = Color.cyan;
    // 非選択時のフレームの色
    public Color normalFrameColor = Color.white;
    // スタック数
    public Text stackCountText;
    // 対応するインベントリスロット
    public InventorySlot slot;
    // このスロット番号
    public int slotIndex;
    // 親UI
    private InventoryUI parentUI;

    // 初期化
    public void Initialize(InventoryUI ui, int index, InventorySlot targetSlot)
    {
        parentUI = ui;
        slotIndex = index;
        slot = targetSlot;

    }

    /// <summary>
    /// スロットを更新してアイコン表示
    /// </summary>
    public void Refresh()
    {

        if (slot != null && slot.tileData != null && icon != null)
        {
            icon.sprite = slot.tileData.Icon;
            icon.enabled = true;
            if (slot.stackCount > 1) stackCountText.text = slot.stackCount.ToString();
            else stackCountText.text = "";
        }
        else if (icon != null)
        {
            icon.enabled = false;
            stackCountText.text = "";
        }
    }




}
