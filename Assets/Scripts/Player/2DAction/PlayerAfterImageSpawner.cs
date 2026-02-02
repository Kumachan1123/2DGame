using UnityEngine;

/// <summary>
/// プレイヤーの残像生成クラス
/// </summary>
public class PlayerAfterImageSpawner : MonoBehaviour
{
    /// <summary>
    /// 残像プレハブ
    /// </summary>
    [SerializeField] private GameObject m_afterImagePrefab;

    /// <summary>
    /// 残像の色
    /// </summary>
    [SerializeField] private Color m_afterImageColor = Color.cyan;

    /// <summary>
    /// 残像を出す間隔
    /// </summary>
    [SerializeField] private float m_spawnInterval = 0.05f;

    /// <summary>
    /// 前回生成した時間
    /// </summary>
    private float m_timer = 0.0f;

    /// <summary>
    /// スプライトレンダラー
    /// </summary>
    private SpriteRenderer m_spriteRenderer;

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        m_timer += Time.deltaTime;

        if (m_timer >= m_spawnInterval)
        {
            SpawnAfterImage();
            m_timer = 0.0f;
        }
    }

    /// <summary>
    /// 残像を生成する
    /// </summary>
    private void SpawnAfterImage()
    {
        GameObject obj = Instantiate(m_afterImagePrefab, transform.position, transform.rotation);
        Debug.Log("残像を生成しました");

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        sr.sprite = m_spriteRenderer.sprite;
        sr.flipX = m_spriteRenderer.flipX;
        sr.color = m_afterImageColor;
    }
}
