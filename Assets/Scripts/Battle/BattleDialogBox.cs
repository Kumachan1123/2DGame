using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// ダイアログのテキストを取得して変更する
public class BattleDialogBox : MonoBehaviour
{
    // ダイアログテキスト
    [SerializeField] Text m_dialogText;
    // 一文字当たりの表示速度
    [SerializeField, Header("1文字あたりの表示速度")] int m_lettersPerSecond;

    // テキストを変更するための関数
    public void SetDialog(string dialog)
    {
        m_dialogText.text = dialog;
    }
    // テキストを一文字ずつ表示するコルーチン
    public IEnumerator TypeDialog(string dialog)
    {
        m_dialogText.text = "";
        foreach (char letter in dialog)
        {
            m_dialogText.text += letter;
            yield return new WaitForSeconds(1f / m_lettersPerSecond);
        }
    }
}
