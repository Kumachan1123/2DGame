using UnityEngine;

/// <summary>
/// ダッシュ中のみ残像を生成するコンポーネント
/// </summary>
public class AfterImageEmitter : MonoBehaviour
{
    /// <summary>
    /// 残像プレハブ
    /// </summary>
    [SerializeField]
    private GameObject m_afterImagePrefab;

    /// <summary>
    /// 残像の色
    /// </summary>
    [SerializeField]
    private Color m_afterImageColor = Color.cyan;

    /// <summary>
    /// 残像生成間隔
    /// </summary>
    [SerializeField]
    private float m_spawnInterval = 0.05f;

    /// <summary>
    /// スプライトレンダラー
    /// </summary>
    private SpriteRenderer m_spriteRenderer;

    /// <summary>
    /// 残像生成タイマー
    /// </summary>
    private float m_timer;

    /// <summary>
    /// 残像を出すかどうか
    /// </summary>
    private bool m_isEmitting;

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!m_isEmitting) return;

        m_timer += Time.deltaTime;

        if (m_timer >= m_spawnInterval)
        {
            SpawnAfterImage();
            m_timer = 0f;
        }
    }

    /// <summary>
    /// 残像生成を開始
    /// </summary>
    public void StartEmit()
    {
        m_isEmitting = true;
        m_timer = 0f;
    }

    /// <summary>
    /// 残像生成を停止
    /// </summary>
    public void StopEmit()
    {
        m_isEmitting = false;
    }

    /// <summary>
    /// 残像を生成
    /// </summary>
    private void SpawnAfterImage()
    {
        GameObject obj = Instantiate(m_afterImagePrefab, transform.position, transform.rotation);

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        sr.sprite = m_spriteRenderer.sprite;
        sr.flipX = m_spriteRenderer.flipX;
        sr.color = m_afterImageColor;
    }
}
