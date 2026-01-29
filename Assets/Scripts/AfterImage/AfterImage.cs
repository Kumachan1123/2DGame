using UnityEngine;

/// <summary>
/// 残像を時間経過でフェードアウトさせるクラス
/// </summary>
public class AfterImage : MonoBehaviour
{
    /// <summary>
    /// フェードアウトにかかる時間
    /// </summary>
    [SerializeField] private float m_fadeTime = 0.3f;

    /// <summary>
    /// 残像の初期色
    /// </summary>
    [SerializeField]
    private Color m_afterImageColor = new Color(0.3f, 1.2f, 1.2f, 0.8f);


    /// <summary>
    /// スプライトレンダラー
    /// </summary>
    private SpriteRenderer m_spriteRenderer;

    /// <summary>
    /// 経過時間
    /// </summary>
    private float m_timer = 0.0f;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        // 生成時に一度だけ色をセット
        m_spriteRenderer.color = m_afterImageColor;
    }

    /// <summary>
    /// 毎フレーム更新
    /// </summary>
    private void Update()
    {
        m_timer += Time.deltaTime;

        float t = m_timer / m_fadeTime;

        Color color = m_spriteRenderer.color;
        color.a = Mathf.Lerp(m_afterImageColor.a, 0.0f, t);
        m_spriteRenderer.color = color;

        if (m_timer >= m_fadeTime)
        {
            Destroy(gameObject);
        }
    }
}
