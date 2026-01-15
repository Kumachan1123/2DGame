using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// ダイアログのテキストを取得して変更する
public class BattleDialogBox : MonoBehaviour
{
    // 選ばれたアクションの色
    [SerializeField, Header("選ばれたアクションの色")] Color m_selectedActionColor;
    // ダイアログテキスト
    [SerializeField] Text m_dialogText;
    // 一文字当たりの表示速度
    [SerializeField, Header("1文字あたりの表示速度")] int m_lettersPerSecond;
    // アクションセレクター
    [SerializeField, Header("アクションセレクター")] GameObject m_actionSelector;
    // 行動セレクター
    [SerializeField, Header("行動セレクター")] GameObject m_moveSelector;
    // 行動の詳細
    [SerializeField, Header("行動の詳細")] GameObject m_moveDetails;
    // アクションテキスト
    [SerializeField] List<Text> m_actionTexts;
    // 行動テキスト
    [SerializeField] List<Text> m_moveTexts;
    [SerializeField, Header("PPテキスト")] Text m_ppText;
    [SerializeField, Header("タイプテキスト")] Text m_typeText;


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

    // UIの表示・非表示をする
    // ダイアログテキストの表示管理
    public void EnableDialogText(bool enabled)
    {
        m_dialogText.enabled = enabled;
    }
    // UIの表示・非表示をする
    // アクションセレクターの表示管理
    public void EnableActionSelector(bool enabled)
    {
        m_actionSelector.SetActive(enabled);
    }
    // UIの表示・非表示をする
    // 行動セレクターと行動詳細の表示管理
    public void EnableMoveSelector(bool enabled)
    {
        m_moveSelector.SetActive(enabled);
        m_moveDetails.SetActive(enabled);
    }
    // 選択中のアクションの色を変える
    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < m_actionTexts.Count; i++)
        {
            if (i == selectedAction) m_actionTexts[i].color = m_selectedActionColor;
            else m_actionTexts[i].color = Color.black;
        }
    }
    // 選択中の技の色を変える
    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < m_moveTexts.Count; i++)
        {
            if (i == selectedMove) m_moveTexts[i].color = m_selectedActionColor;
            else m_moveTexts[i].color = Color.black;
        }
        // 技の詳細を反映
        m_ppText.text = $"PP {move.PP}/{move.Base.MaxPP}";
        m_typeText.text = GetMoveName(move.Base.Type);
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < m_moveTexts.Count; i++)
        {
            // 覚えている数だけ反映
            if (i < moves.Count) m_moveTexts[i].text = moves[i].Base.Name;
            else m_moveTexts[i].text = "---";
        }
    }


    // move.Base.Type.ToString()は英語なので日本語にする
    string GetMoveName(MonsterType type)
    {
        switch (type)
        {
            case MonsterType.None:
                return "なし";
            case MonsterType.Normal:
                return "ノーマル";
            case MonsterType.Fire:
                return "ほのお";
            case MonsterType.Water:
                return "みず";
            case MonsterType.Grass:
                return "くさ";
            case MonsterType.Electric:
                return "でんき";
            case MonsterType.Ice:
                return "こおり";
            case MonsterType.Fighting:
                return "かくとう";
            case MonsterType.Poison:
                return "どく";
            case MonsterType.Ground:
                return "じめん";
            case MonsterType.Flying:
                return "ひこう";
            case MonsterType.Psychic:
                return "エスパー";
            case MonsterType.Bug:
                return "むし";
            case MonsterType.Rock:
                return "いわ";
            case MonsterType.Ghost:
                return "ゴースト";
            case MonsterType.Dragon:
                return "ドラゴン";
            case MonsterType.Dark:
                return "あく";
            case MonsterType.Steel:
                return "はがね";
            case MonsterType.Fairy:
                return "フェアリー";
            default:
                return "";
        }
    }
}
