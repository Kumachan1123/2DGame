/*
*       SubUIMover.cs
*       UIMoverで最初に動かしたUIが動き終わった後に動かすUIの処理を行う
 */

using UnityEngine;
using DG.Tweening;
using System.Collections;

public class SubUIMover : MonoBehaviour
{
    // 移動先の位置
    [Header("移動先の位置")]
    [SerializeField]
    private Vector3 m_targetPosition;
    // 移動にかける時間
    [Header("移動にかける時間")]
    [SerializeField]
    private float m_duration;
    // 矩形トランスフォームの参照
    private RectTransform m_rectTransform;
    // 演出開始フラグ
    private bool m_startPerformance = false;
    // 演出終了フラグ
    private bool m_performanceFinished = false;

    /// <summary>
    /// 開始処理
    /// </summary>
    private void Start()
    {
        // RectTransformを取得
        m_rectTransform = GetComponent<RectTransform>();

    }

    private void Update()
    {
        if (m_startPerformance)
        {
            m_startPerformance = false;

            // 演出開始
            StartCoroutine(PerformanceFlow());
        }
    }

    private IEnumerator PerformanceFlow()
    {
        bool performanceFinished = false;
        // 目標位置まで移動
        m_rectTransform.DOAnchorPos(m_targetPosition, m_duration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            performanceFinished = true;
        });
        // フェードイン終わるまで待つ
        yield return new WaitUntil(() => performanceFinished);

        yield return new WaitForSeconds(1.0f);
        // 演出終了フラグを立てる
        m_performanceFinished = true;
    }

    public bool StartPerformance
    {
        get => m_startPerformance;
        set => m_startPerformance = value;
    }

    public bool PerformanceFinished
    {
        get => m_performanceFinished;
    }
}
