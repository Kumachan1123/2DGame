/*
 *      TimerCount.cs
 *      ゲーム内時間をUIに表示する
*/
using UnityEngine;
using UnityEngine.UI;

public class TimerCount : MonoBehaviour
{
    // UIテキスト
    private Text m_timerText;
    // カウント中フラグ
    private bool m_isRunning = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // UIテキストを取得
        m_timerText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // ゲーム内時間を取得
        if (m_timerText != null && m_isRunning)
        {
            // 時間を分と秒に変換
            float time = Time.timeSinceLevelLoad;
            int minutes = (int)(time / 60);
            int seconds = (int)(time % 60);
            // テキストに表示
            m_timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

    }

    public bool IsRunning
    {
        get { return m_isRunning; }
        set { m_isRunning = value; }
    }
}
