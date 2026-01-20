using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの入力から採掘と設置を行うクラス
/// </summary>
[RequireComponent(typeof(PlayerInventory))]
public class PlayerInteractor : MonoBehaviour
{
    /// <summary>
    /// 操作対象のTilemap
    /// </summary>
    public Tilemap groundTilemap;

    /// <summary>
    /// 使用するタイル定義の一覧 (スクリプタブルオブジェクト)
    /// </summary>
    public TileDefinitions tileDefinitionsSO;

    /// <summary>
    /// TileDataの定義一覧
    /// </summary>
    private List<TileData> tileDefinitions = new List<TileData>();

    /// <summary>
    /// 採掘システム
    /// </summary>
    private MiningSystem miningSystem;

    /// <summary>
    /// 設置システム
    /// </summary>
    private PlacementSystem placementSystem;

    /// <summary>
    /// プレイヤーのインベントリ
    /// </summary>
    private PlayerInventory inventory;

    /// <summary>
    /// 起動時処理
    /// </summary>
    private void Awake()
    {
        // スクリプタブルオブジェクトからタイル定義を取得
        tileDefinitions = tileDefinitionsSO.tileDefinitions;

        // インベントリコンポーネントを取得
        inventory = GetComponent<PlayerInventory>();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        // MiningSystem を生成
        miningSystem = new MiningSystem(groundTilemap, tileDefinitions);

        // PlacementSystem を生成
        placementSystem = new PlacementSystem(groundTilemap, inventory);
    }

    /// <summary>
    /// 毎フレーム入力を監視する
    /// </summary>
    private void Update()
    {
        // 採掘
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryMineAtMousePosition();
        }

        // 設置
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            TryPlaceAtMousePosition();
        }
    }

    /// <summary>
    /// マウス位置のタイルを採掘する
    /// </summary>
    private void TryMineAtMousePosition()
    {
        // マウスのスクリーン座標を取得
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        // ワールド座標に変換
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        // セル座標に変換
        Vector3Int cellPos = groundTilemap.WorldToCell(mouseWorldPos);

        // 採掘を実行
        bool result = miningSystem.Mine(cellPos);

        // デバッグ表示
        if (result)
        {
            Debug.Log("Mine Success : " + cellPos);
        }
        else
        {
            Debug.Log("Mine Failed : " + cellPos);
        }
    }

    /// <summary>
    /// マウス位置にタイルを設置する
    /// </summary>
    private void TryPlaceAtMousePosition()
    {
        // マウスのスクリーン座標を取得
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        // ワールド座標に変換
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        // セル座標に変換
        Vector3Int cellPos = groundTilemap.WorldToCell(mouseWorldPos);

        // 選択中のタイルを取得
        TileData selectedTile = inventory.GetSelectedTile();
        if (selectedTile != null)
        {
            // 設置処理を呼ぶ
            placementSystem.Place(cellPos, selectedTile);
        }
    }
}
