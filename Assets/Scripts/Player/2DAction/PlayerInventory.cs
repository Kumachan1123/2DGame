using System;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class InventorySlot
{
    public TileData tileData; // スロットに入っているアイテム
    public int stackCount;    // そのアイテムの数（最大100など）

    public bool IsEmpty => tileData == null || stackCount <= 0;
    public TileData Item => tileData;
}

public class PlayerInventory : MonoBehaviour
{
    public int slotCount = 45;
    public InventorySlot[] slots;
    public int selectedSlotIndex = 0;
    public InventoryUI inventoryUI;

    public InventoryUI InventoryUI => inventoryUI;

    /// <summary>
    /// Inventory に変更があったときに発火するイベント
    /// </summary>
    public event Action OnInventoryChanged;

    private void Awake()
    {
        slots = new InventorySlot[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            slots[i] = new InventorySlot { tileData = null, stackCount = 0 };
        }
    }

    public bool AddItem(TileData tile, int amount = 1)
    {
        bool changed = false;

        // 既存のスタックに追加
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].tileData == tile && slots[i].stackCount < 100)
            {
                int space = 100 - slots[i].stackCount;
                int toAdd = Mathf.Min(space, amount);
                slots[i].stackCount += toAdd;
                amount -= toAdd;
                changed = true;
                if (amount <= 0) break;
            }
        }

        // 空スロットに入れる
        for (int i = 0; i < slots.Length && amount > 0; i++)
        {
            if (slots[i].tileData == null)
            {
                slots[i].tileData = tile;
                slots[i].stackCount = Mathf.Min(100, amount);
                amount -= slots[i].stackCount;
                changed = true;
            }
        }

        if (changed) OnInventoryChanged?.Invoke();
        return amount <= 0;
    }

    public TileData GetSelectedTile()
    {
        if (selectedSlotIndex < 0 || selectedSlotIndex >= slots.Length) return null;
        return slots[selectedSlotIndex].tileData;
    }

    public void ConsumeSelectedTile(int amount = 1)
    {
        if (selectedSlotIndex < 0 || selectedSlotIndex >= slots.Length) return;

        InventorySlot slot = slots[selectedSlotIndex];
        if (slot.tileData == null) return;

        slot.stackCount -= amount;
        if (slot.stackCount <= 0)
        {
            slot.tileData = null;
            slot.stackCount = 0;
        }

        OnInventoryChanged?.Invoke();
    }

    private void Update()
    {
        // マウスのスクロールで選択スロットを変更
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0f) SelectPrevious();
        else if (scroll < 0f) SelectNext();
    }
    /// <summary>
    /// 前のスロットを選択
    /// </summary>
    public void SelectNext()
    {

        selectedSlotIndex++;
        if (selectedSlotIndex >= slots.Length) selectedSlotIndex = 0;
    }

    /// <summary>
    /// 次のスロットを選択
    /// </summary>
    private void SelectPrevious()
    {
        selectedSlotIndex--;
        if (selectedSlotIndex < 0) selectedSlotIndex = slots.Length - 1;
    }
}
