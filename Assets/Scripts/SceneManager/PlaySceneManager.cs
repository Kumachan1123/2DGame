using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlaySceneManager : MonoBehaviour
{
    // フェード管理クラス
    [SerializeField, Header("フェード")]
    private UIFade m_fade;
    // インプットレシーバー
    [SerializeField, Header("インプットレシーバー")]
    private PlayInputReceiver m_inputReceiver;
    // 遷移先のシーン
    [SerializeField, Header("遷移先のシーン")]
    private string m_nextSceneName = "PaletteSelectScene";
    // ゴールオブジェクト
    [SerializeField, Header("ゴールオブジェクト")]
    private Goal m_goalObject;
    // 動かすUI
    [SerializeField, Header("動かすUI")]
    private UIMover m_uiMover;
    // タイマーUI
    [SerializeField, Header("タイマーUI")]
    private TimerCount m_timerCount;

    // ゴール処理がされたか
    private bool m_isGoalProcessed = false;
    // クリア演出が終わったか
    private bool m_isClearPerformanceDone = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // インプットレシーバー取得
        m_inputReceiver = GetComponent<PlayInputReceiver>();


        // UIムーバーの完了イベントに登録
        m_uiMover.OnMoveComplete += OnUIMoveFinished;
        m_fade.FadeInWithCallback(() =>
        {

        });
    }

    // Update is called once per frame
    void Update()
    {
        if (m_inputReceiver.GetInputButton(PlayInputReceiver.Actions.EXIT, PlayInputReceiver.InputType.PRESSED))
        {
            // アプリケーション終了
            StartCoroutine(ExitGame(0.5f));
        }
        // ゴールに触れたら演出開始だけ
        if (m_goalObject.IsTouched)
        {
            m_uiMover.StartPerformance = true;
            // タイマー停止
            m_timerCount.IsRunning = false;
        }
    }
    // 追加：ゲームそのものを終了する（遅延実行）
    private IEnumerator ExitGame(float duration)
    {
        yield return new WaitForSeconds(duration);
        m_fade.FadeOutWithCallback(() =>
        {
            // アプリケーション終了
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        });
    }
    // 追加：セレクト画面に移動（遅延実行）
    private IEnumerator EnterToSelectScene(float duration)
    {
        yield return new WaitForSeconds(duration);
        m_fade.FadeOutWithCallback(() =>
        {
            // シーンを変える
            SceneManager.LoadScene(m_nextSceneName);
        });
    }
    /// <summary>
    /// UIの移動が完了したときの処理
    /// UIMoverからInvokeされる
    /// </summary>
    private void OnUIMoveFinished()
    {
        // もう処理済みなら何もしない
        if (m_isGoalProcessed) return;

        m_isGoalProcessed = true;
        StartCoroutine(EnterToSelectScene(3.0f));
    }

    private void OnDestroy()
    {
        if (m_uiMover != null)
        {
            m_uiMover.OnMoveComplete -= OnUIMoveFinished;
        }
    }

}
