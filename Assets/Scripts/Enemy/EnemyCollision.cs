using System.Collections;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    // スプライトレンダラー
    private SpriteRenderer m_spriteRenderer;
    // アニメーター
    private Animator m_animator;
    // 敵スクリプト
    private Enemy m_enemy;


    private void Start()
    {
        // コンポーネントの取得
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_animator = GetComponent<Animator>();
        m_enemy = GetComponent<Enemy>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // プレイヤーに上から踏まれたら
            if (collision.contacts[0].normal.y < -0.5f)
            {
                // プレイヤーをジャンプさせる
                collision.gameObject.GetComponent<ActionPlayer>().Jump();
                // 敵を上下反転させる
                m_spriteRenderer.flipY = true;
                // アニメーションを死亡アニメーションに切り替え
                m_animator.SetBool("isDead", true);
                // 敵の動きを止める
                m_enemy.IsDead = true;
                // 敵を倒す処理
                StartCoroutine(Deth());
            }
            // 上以外から当たったら
            else
            {
                // プレイヤーにダメージを与える
                PlayerHP playerHP = collision.gameObject.GetComponent<PlayerHP>();
                if (playerHP != null) playerHP.TakeDamage();
            }

        }
    }

    private IEnumerator Deth()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);

    }
}
