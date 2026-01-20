using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの入力から採掘を行うクラス
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    /// <summary>
    /// 操作対象のTilemap
    /// </summary>
    public Tilemap groundTilemap;
    /// <summary>
    /// 使用するタイル定義の一覧(スクリプタブルオブジェクト）
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
        /// スクリプタブルオブジェクトからタイル定義を取得
        tileDefinitions = tileDefinitionsSO.tileDefinitions;
        /// インベントリコンポーネントを取得
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
        placementSystem = new PlacementSystem(groundTilemap);
    }


    /// <summary>
    /// 毎フレーム入力を監視する
    /// </summary>
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryMineAtMousePosition();
        }

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
        // 左クリックされた瞬間だけ処理する
        if (Mouse.current.leftButton.wasPressedThisFrame == false)
        {
            return;
        }

        // マウスのスクリーン座標を取得
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        /// ワールド座標に変換
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        /// Z座標を0に補正
        mouseWorldPos.z = 0f;

        /// セル座標に変換
        Vector3Int cellPos = groundTilemap.WorldToCell(mouseWorldPos);

        /// 採掘を実行
        bool result = miningSystem.Mine(cellPos);

        /// デバッグ表示
        if (result)
        {
            Debug.Log("Mine Success : " + cellPos);
        }
        else
        {
            Debug.Log("Mine Failed : " + cellPos);
        }
    }

    private void TryPlaceAtMousePosition()
    {
        if (inventory.currentTile == null)
        {
            return;
        }

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3Int cellPos = groundTilemap.WorldToCell(worldPos);

        bool result = placementSystem.Place(cellPos, inventory.currentTile);

        if (result)
        {
            /// 成功したらインベントリから消す
            inventory.currentTile = null;
        }
    }

}
