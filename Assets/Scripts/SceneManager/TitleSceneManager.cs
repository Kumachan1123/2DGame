using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    // フェード管理クラス
    [SerializeField, Header("フェード")]
    private UIFade m_fade;
    // インプットレシーバー
    [SerializeField, Header("インプットレシーバー")]
    private TitleInputReceiver m_inputReceiver;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_inputReceiver = GetComponent<TitleInputReceiver>();
        m_fade.FadeInWithCallback(() =>
        {

        });
    }

    // Update is called once per frame
    void Update()
    {
        if (m_inputReceiver.GetInputButton(TitleInputReceiver.Actions.EXIT, TitleInputReceiver.InputType.PRESSED))
        {
            // アプリケーション終了
            StartCoroutine(ExitGame(0.5f));
        }
        if (m_inputReceiver.GetInputButton(TitleInputReceiver.Actions.ENTER, TitleInputReceiver.InputType.PRESSED))
        {
            StartCoroutine(EnterToSelectScene(.1f));
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
        { // シーンを変える
            SceneManager.LoadScene("SelectScene");
        });
    }
}
