/*
*       UIMover.cs
*       DOTweenを使って指定の位置までUIを動かす
*/
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class UIMover : MonoBehaviour
{
    [Header("移動先の位置")]
    [SerializeField]
    private Vector3 m_targetPosition;

    [Header("移動にかける時間")]
    [SerializeField]
    private float m_duration = 1.0f;

    // 移動終了のコールバック
    public System.Action OnMoveComplete;
    // 演出開始フラグ
    private bool m_startPerformance = false;
    // 移動中フラグ
    private bool m_isMoving = false;
    // 移動完了フラグ
    private bool m_isCompleted = false;
    // 矩形トランスフォームの参照
    private RectTransform m_rectTransform;


    // Start is called before the first frame update
    void Start()
    {
        // RectTransformを取得
        m_rectTransform = GetComponent<RectTransform>();


    }

    private void Update()
    {
        if (m_startPerformance)
        {
            m_startPerformance = false;
            StartCoroutine(PerformanceFlow());
        }
    }

    /// <summary>
    /// 演出の一連の流れ
    /// </summary>
    private IEnumerator PerformanceFlow()
    {
        m_isMoving = false;
        m_isCompleted = false;

        // ① 最初の3秒待機
        yield return new WaitForSeconds(3f);

        // ② 移動開始
        m_isMoving = true;

        bool tweenFinished = false;

        m_rectTransform.DOAnchorPos(m_targetPosition, m_duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                tweenFinished = true;
            });

        // Tween終わるまで待つ
        yield return new WaitUntil(() => tweenFinished);

        m_isMoving = false;

        // ③ 到着後3秒待機
        yield return new WaitForSeconds(3f);

        // ④ 完了通知
        m_isCompleted = true;
        OnMoveComplete?.Invoke();
    }



    public bool StartPerformance
    {
        get => m_startPerformance;
        set => m_startPerformance = value;
    }
    public bool IsMoving => m_isMoving;

    public bool IsCompleted => m_isCompleted;

}
