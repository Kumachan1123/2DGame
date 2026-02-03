/*
*       UIMover.cs
*       DOTweenを使って指定の位置までUIを動かす
*/
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMover : MonoBehaviour
{
    [Header("移動先の位置")]
    [SerializeField]
    private Vector3 m_targetPosition;

    [Header("移動にかける時間")]
    [SerializeField]
    private float m_duration;

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
    // UIテキスト
    private Text m_uiText;
    // このUIが移動完了した後に動かしたいUIの参照リスト
    [Header("このUIが移動完了した後に動かしたいUIの参照リスト")]
    [SerializeField]
    private SubUIMover[] m_nextUIMovers;

    // Start is called before the first frame update
    void Start()
    {
        // RectTransformを取得
        m_rectTransform = GetComponent<RectTransform>();
        // UIテキストを取得
        m_uiText = GetComponent<Text>();
        // 文字の色を透過
        m_uiText.color = new Color(m_uiText.color.r, m_uiText.color.g, m_uiText.color.b, 0f);


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

    /// <summary>
    /// 演出の一連の流れ
    /// </summary>
    private IEnumerator PerformanceFlow()
    {
        m_isMoving = false;
        m_isCompleted = false;

        // ① 文字をフェードイン
        float fadeDuration = 0.25f;
        bool fadeFinished = false;

        m_uiText.DOFade(1f, fadeDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                fadeFinished = true;
            });

        // フェードイン終わるまで待つ
        yield return new WaitUntil(() => fadeFinished);

        // ★ここで3秒停止したい！
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

        // 移動が終わるまで待つ
        yield return new WaitUntil(() => tweenFinished);

        m_isMoving = false;

        // ③ 到着後3秒待機
        yield return new WaitForSeconds(3f);

        // ④ 完了通知
        m_isCompleted = true;
        // イベント発火
        // もし次に動かすUIがあれば動かす
        if (m_nextUIMovers != null)
        {
            foreach (var uiMover in m_nextUIMovers)
            {
                uiMover.StartPerformance = true;
            }
        }
        // サブUIの移動が完了したらイベント発火
        if (m_nextUIMovers != null)
        {
            foreach (var uiMover in m_nextUIMovers)
            {
                // 一個でもFalseなら待つ
                if (!uiMover.PerformanceFinished)
                {
                    yield return new WaitUntil(() => uiMover.PerformanceFinished);
                }
            }
            // 全てのサブUIの移動が完了したらイベント発火
            // サブUIがなければ即座にイベント発火
            Debug.Log("UIMover: OnMoveComplete Invoked");
            OnMoveComplete?.Invoke();
        }
        else
        {
            // サブUIがなければ即座にイベント発火
            Debug.Log("UIMover: OnMoveComplete Invoked");
            OnMoveComplete?.Invoke();
        }

    }



    public bool StartPerformance
    {
        get => m_startPerformance;
        set => m_startPerformance = value;
    }
    public bool IsMoving => m_isMoving;

    public bool IsCompleted => m_isCompleted;

}
