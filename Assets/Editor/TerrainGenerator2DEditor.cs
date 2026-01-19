using UnityEngine;
using UnityEditor;

/// <summary>
/// TerrainGenerator2D 用のカスタムInspector
/// </summary>
[CustomEditor(typeof(TerrainGenerator2D))]
public class TerrainGenerator2DEditor : Editor
{
    /// <summary>
    /// Inspectorの描画処理
    /// </summary>
    public override void OnInspectorGUI()
    {
        /// デフォルトのInspectorをそのまま描画
        DrawDefaultInspector();

        /// 対象のコンポーネントを取得
        TerrainGenerator2D generator = (TerrainGenerator2D)target;

        GUILayout.Space(10);

        /// Generateボタンを表示
        if (GUILayout.Button("Generate Terrain"))
        {
            /// Undo対応
            Undo.RegisterCompleteObjectUndo(generator.groundTilemap, "Generate Terrain");

            /// 地形生成を実行
            generator.GenerateFromEditor();

            /// Tilemapの変更を保存対象にする
            EditorUtility.SetDirty(generator.groundTilemap);
        }
    }
}
