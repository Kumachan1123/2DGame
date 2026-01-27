using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("敵の歩行速度")]
    [SerializeField]
    private float m_walkSpeed = 1.0f;

    [Header("地面の端にいるときに向きを変えるか")]
    [SerializeField]
    private bool m_turnAtEdge = true;

    [Header("地面判定用")]
    [SerializeField]
    private float m_groundCheckDistance = 0.5f;

    [Header("地面判定のオフセット（敵の足元からの距離）")]
    [SerializeField]
    private float m_edgeCheckOffset = 0.3f;

    [Header("壁判定の距離")]
    [SerializeField]
    private float m_wallCheckDistance = 0.3f;

    // 物理演算の参照
    private Rigidbody2D m_rigidbody2D;
    // スプライトの参照
    private SpriteRenderer m_spriteRenderer;
    // 敵の向き
    private bool m_facingRight = true;
    // 連続反転防止用
    private float m_turnCooldown = 0f;
    // 死んだか
    private bool m_isDead = false;

    void Start()
    {
        // Rigidbody2Dを取得
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        // SpriteRendererを取得
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        // 初期化時は少し待つ
        m_turnCooldown = 0.5f;
    }

    void Update()
    {
        // クールダウン更新
        if (m_turnCooldown > 0f)
        {
            m_turnCooldown -= Time.deltaTime;
        }

        // 歩行処理
        if (!m_isDead) Walk();

        // 地面の端チェック
        if (m_turnAtEdge && m_turnCooldown <= 0f)
        {
            if (CheckEdge())
            {
                Turn();
            }
            else if (CheckWall())
            {
                Turn();
            }
        }
    }

    /// <summary>
    /// 敵を現在の向きに歩かせる
    /// </summary>
    private void Walk()
    {
        float moveDir = m_facingRight ? 1f : -1f;

        // X方向のみ速度を設定
        m_rigidbody2D.linearVelocity = new Vector2(
            moveDir * m_walkSpeed,
            m_rigidbody2D.linearVelocity.y
        );
    }

    /// <summary>
    /// 壁や地面の端で向きを反転させる
    /// </summary>
    private void Turn()
    {
        // 向きを反転
        m_facingRight = !m_facingRight;

        // スプライトを反転
        m_spriteRenderer.flipX = !m_spriteRenderer.flipX;

        // クールダウンを設定（0.3秒間は再度反転しない）
        m_turnCooldown = 0.3f;

        Debug.Log($"敵が反転しました！ 向き: {(m_facingRight ? "右" : "左")}");
    }

    /// <summary>
    /// 地面の端にいるかチェック
    /// </summary>
    private bool CheckEdge()
    {
        // 足元の少し前の位置から下にRayを飛ばす
        Vector2 origin = (Vector2)transform.position + new Vector2(
            m_facingRight ? m_edgeCheckOffset : -m_edgeCheckOffset,
            0f
        );

        // すべてのレイヤーに対して判定（敵自身は除外）
        int layerMask = ~(1 << gameObject.layer);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, m_groundCheckDistance, layerMask);

        // デバッグ用
        Debug.DrawRay(origin, Vector2.down * m_groundCheckDistance, hit.collider ? Color.green : Color.red);

        // 地面がなかったら true を返す
        return !hit.collider;
    }

    /// <summary>
    /// 壁があるかチェック
    /// </summary>
    private bool CheckWall()
    {
        Vector2 origin = transform.position;
        Vector2 dir = m_facingRight ? Vector2.right : Vector2.left;

        // すべてのレイヤーに対して判定（敵自身は除外）
        int layerMask = ~(1 << gameObject.layer);
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, m_wallCheckDistance, layerMask);

        // デバッグ用
        Debug.DrawRay(origin, dir * m_wallCheckDistance, hit.collider ? Color.blue : Color.yellow);

        // 壁があって、それがGroundタグなら true を返す
        return hit.collider != null && hit.collider.CompareTag("Ground");
    }

#if UNITY_EDITOR
    /// <summary>
    /// エディタ上でRayの可視化
    /// </summary>
    private void OnDrawGizmos()
    {
        // 地面判定のRay
        Gizmos.color = Color.red;
        Vector2 edgeOrigin = (Vector2)transform.position + new Vector2(
            m_facingRight ? m_edgeCheckOffset : -m_edgeCheckOffset,
            0f
        );
        Gizmos.DrawLine(edgeOrigin, edgeOrigin + Vector2.down * m_groundCheckDistance);

        // 壁判定のRay
        Gizmos.color = Color.blue;
        Vector2 wallDir = m_facingRight ? Vector2.right : Vector2.left;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + wallDir * m_wallCheckDistance);
    }
#endif

    // 以下、変数を外部から参照・変更するためのプロパティ
    public bool IsDead { get => m_isDead; set => m_isDead = value; }
}