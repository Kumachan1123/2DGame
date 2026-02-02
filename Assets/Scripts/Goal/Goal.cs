/*
*       Goal.cs
*       ゴールのオブジェクトにプレイヤーが触ったら発生する処理
*/
using UnityEngine;
public class Goal : MonoBehaviour
{
    // プレイヤーに触れられたか
    private bool m_isTouched = false;
    // 縮小開始フラグ
    private bool m_startScaleDown = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーがゴールに触れたら
        if (other.CompareTag("Player"))
        {
            // ゴール処理を実行
            Debug.Log("ゴールしました！");
            // ここにゴール時の処理を追加（例：シーン遷移、スコア表示など）
            // ゴールオブジェクトを徐々に小さくする処理を開始
            m_startScaleDown = true;
        }
    }

    void Update()
    {
        if (m_startScaleDown)
        {
            HandleScaleDown();
            // 一定以下の大きさになったら触れられたことにする
            if (transform.localScale.x < 0.01f)
            {
                m_isTouched = true;
                m_startScaleDown = false;
            }
        }
    }

    private void HandleScaleDown()
    {
        // ゴールオブジェクトを徐々に小さくする処理
        float scaleFactor = 0.9f; // 縮小率
        transform.localScale *= scaleFactor;
    }

    // プレイヤーに触れられたかを取得するプロパティ
    public bool IsTouched { get => m_isTouched; }
}
