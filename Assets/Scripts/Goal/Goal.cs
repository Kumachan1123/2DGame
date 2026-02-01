/*
*       Goal.cs
*       ゴールのオブジェクトにプレイヤーが触ったら発生する処理
*/
using UnityEngine;
public class Goal : MonoBehaviour
{
    // プレイヤーに触れられたか
    private bool m_isTouched = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーがゴールに触れたら
        if (other.CompareTag("Player"))
        {
            // ゴール処理を実行
            Debug.Log("ゴールしました！");
            // ここにゴール時の処理を追加（例：シーン遷移、スコア表示など）
            // 触れられたことにする
            m_isTouched = true;
        }
    }
    // プレイヤーに触れられたかを取得するプロパティ
    public bool IsTouched { get => m_isTouched; }
}
