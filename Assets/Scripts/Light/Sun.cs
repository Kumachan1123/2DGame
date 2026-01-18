//      Sun.cs
//      2D画面全体の光源を管理するスクリプト
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Sun : MonoBehaviour
{
    // ライトコンポーネント
    private Light2D m_light;
    // 朝の時の光の色
    [SerializeField, Header("朝の光の色")]
    private Color m_morningColor = new Color(1f, 0.956f, 0.839f);
    // 昼の時の光の色
    [SerializeField, Header("昼の光の色")]
    private Color m_noonColor = Color.white;
    // 夕方の時の光の色
    [SerializeField, Header("夕方の光の色")]
    private Color m_eveningColor = new Color(1f, 0.647f, 0.376f);
    // 夜の時の光の色
    [SerializeField, Header("夜の光の色")]
    private Color m_nightColor = new Color(0.2f, 0.2f, 0.35f);
    // 夜から朝に変わるときの光の色
    [SerializeField, Header("夜明けの光の色")]
    private Color m_nightToMorningColor = new Color(0.5f, 0.5f, 0.7f);

    // ゲーム内時間
    private float m_timeOfDay = 0f; // 0～24の範囲
    // 一日の長さ
    [SerializeField, Header("一日の長さ（分）")]
    private float m_dayLengthInMinutes = 24f;

    private void Awake()
    {
        m_light = GetComponent<Light2D>();
        if (m_light == null)
        {
            Debug.LogError("Lightコンポーネントがアタッチされていません。");
        }
    }
    // ゲーム内時間に応じて光の色を変更する（24分で1日とする）
    private void Update()
    {
        m_timeOfDay += (24f / (m_dayLengthInMinutes * 60f)) * Time.deltaTime;
        if (m_timeOfDay >= 24f)
        {
            m_timeOfDay = 0f;
        }
        UpdateLightColor();

    }

    /// <summary>
    /// ゲーム内時間に応じてライトの色を更新する
    /// </summary>
    private void UpdateLightColor()
    {
        // 朝 → 昼（6 ～ 12）
        if (m_timeOfDay >= 6f && m_timeOfDay < 12f)
        {
            float t = (m_timeOfDay - 6f) / 6f;
            m_light.color = Color.Lerp(m_morningColor, m_noonColor, t);
            return;
        }

        // 昼 → 夕（12 ～ 18）
        if (m_timeOfDay >= 12f && m_timeOfDay < 18f)
        {
            float t = (m_timeOfDay - 12f) / 6f;
            m_light.color = Color.Lerp(m_noonColor, m_eveningColor, t);
            return;
        }

        // 夕 → 夜（18 ～ 21）
        if (m_timeOfDay >= 18f && m_timeOfDay < 21f)
        {
            float t = (m_timeOfDay - 18f) / 3f;
            m_light.color = Color.Lerp(m_eveningColor, m_nightColor, t);
            return;
        }

        // 夜（21 ～ 24）
        if (m_timeOfDay >= 21f && m_timeOfDay < 24f)
        {
            m_light.color = m_nightColor;
            return;
        }

        // 夜 → 夜明け（0 ～ 3）
        if (m_timeOfDay >= 0f && m_timeOfDay < 3f)
        {
            float t = m_timeOfDay / 3f;
            m_light.color = Color.Lerp(m_nightColor, m_nightToMorningColor, t);
            return;
        }

        // 夜明け → 朝（3 ～ 6）
        float tDawn = (m_timeOfDay - 3f) / 3f;
        m_light.color = Color.Lerp(m_nightToMorningColor, m_morningColor, tDawn);
    }

    // 外部クラスに時間を渡すためのプロパティ
    public float TimeOfDay { get => m_timeOfDay; }
}
