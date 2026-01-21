using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public PlayerInventory inventory;
    public GameObject slotPrefab;
    public Transform slotParent;

    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();

    private void Start()
    {
        // スロットUIを生成
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            GameObject go = Instantiate(slotPrefab, slotParent);
            InventorySlotUI slotUI = go.GetComponent<InventorySlotUI>();
            slotUI.slot = inventory.slots[i];
            slotUIs.Add(slotUI);
        }

        RefreshAll();

        // イベント購読
        inventory.OnInventoryChanged += RefreshAll;
    }

    public void RefreshAll()
    {



        // 全スロットUIを更新
        foreach (var s in slotUIs)
        {
            Debug.Log("SlotUpdate");

            s.Refresh();
            if (s.slot.tileData != null)
            {
                Debug.Log($"このスロットのアイテム：{s.slot.tileData.Name}");
                Debug.Log($"現在の所持数：{s.slot.stackCount}");
            }
        }
    }

    private void OnDestroy()
    {
        // 忘れずにイベント解除
        if (inventory != null)
            inventory.OnInventoryChanged -= RefreshAll;
    }
}
