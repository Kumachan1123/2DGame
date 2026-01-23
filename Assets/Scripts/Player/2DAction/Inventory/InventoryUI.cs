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

            s.Refresh();

        }



    }

    private void Update()
    {
        // 選択中スロットの色更新
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i == inventory.selectedSlotIndex)
                slotUIs[i].frame.color = slotUIs[i].selectedFrameColor;
            else
                slotUIs[i].frame.color = slotUIs[i].normalFrameColor;
        }
    }

    private void OnDestroy()
    {
        // 忘れずにイベント解除
        if (inventory != null)
            inventory.OnInventoryChanged -= RefreshAll;
    }

    /// <summary>
    /// スロット選択処理
    /// </summary>  
    public void SelectSlot(int index)
    {
        inventory.selectedSlotIndex = index;
    }
}
