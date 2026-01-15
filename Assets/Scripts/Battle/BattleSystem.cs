using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public enum BattleState
{
    START,          // バトル開始
    PLAYERACTION,   // プレイヤーのアクション選択
    PLAYERMOVE,     // プレイヤーの技選択
    ENEMYMOVE,      // 敵の技選択
    BUSY            // 処理中
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit m_playerUnit;
    [SerializeField] BattleUnit m_enemyUnit;
    [SerializeField] BattleHud m_playerHud;
    [SerializeField] BattleHud m_enemyHud;
    [SerializeField] BattleDialogBox m_dialogBox;
    int m_currentAction = 0;// 現在のアクション 0:Fight, 1:Run
    public int m_currentMove = 0;// 0:左上　1:右上　2:左下　3:右下
    BattleState m_state;
    // インプットレシーバー
    private BattleInputReceiver m_inputReceiver;

    void Start()
    {
        m_inputReceiver = GetComponent<BattleInputReceiver>();
        // ダイアログを表示
        StartCoroutine(SetUpBattle());
    }

    IEnumerator SetUpBattle()
    {
        // バトルステートを設定
        m_state = BattleState.START;

        m_playerUnit.SetUp();// プレイヤーのモンスターをセットアップ
        m_enemyUnit.SetUp();// 敵のモンスターをセットアップ

        m_playerHud.SetData(m_playerUnit.Monster);// プレイヤーのHUDをセット
        m_enemyHud.SetData(m_enemyUnit.Monster);// 敵のHUDをセット
        // 技の反映
        m_dialogBox.SetMoveNames(m_playerUnit.Monster.Moves);
        yield return m_dialogBox.TypeDialog($"野生の {m_enemyUnit.Monster.Base.Name} が現れた.");
        yield return new WaitForSeconds(1);
        PlayerAction();
    }

    void PlayerAction()
    {
        m_state = BattleState.PLAYERACTION;
        m_dialogBox.EnableActionSelector(true);
        StartCoroutine(m_dialogBox.TypeDialog($" {m_enemyUnit.Monster.Base.Name} はどうする？"));

    }

    void PlayerMove()
    {
        m_state = BattleState.PLAYERMOVE;
        m_dialogBox.EnableDialogText(false);
        m_dialogBox.EnableActionSelector(false);
        m_dialogBox.EnableMoveSelector(true);
    }

    void Update()
    {
        if (m_state == BattleState.PLAYERACTION)
            HandleActionSelection();
        else if (m_state == BattleState.PLAYERMOVE)
            HandleMoveSelection();
    }
    // PlayerActionでの行動
    void HandleActionSelection()
    {
        // 下を入力するとRun、上を入力するとFightになる
        if (m_inputReceiver.GetInputButton(BattleInputReceiver.Actions.DOWN, BattleInputReceiver.InputType.PRESSED))
        {
            m_currentAction++;
        }
        if (m_inputReceiver.GetInputButton(BattleInputReceiver.Actions.UP, BattleInputReceiver.InputType.PRESSED))
        {
            m_currentAction--;
        }
        m_currentAction = m_currentAction < 0 ? 1 : m_currentAction % 2;

        m_dialogBox.UpdateActionSelection(m_currentAction);

        if (m_inputReceiver.GetInputButton(BattleInputReceiver.Actions.SHOW, BattleInputReceiver.InputType.PRESSED))
        {
            PlayerMove();
        }
    }

    void HandleMoveSelection()
    {
        if (m_inputReceiver.GetInputButton(BattleInputReceiver.Actions.RIGHT, BattleInputReceiver.InputType.PRESSED))
        {
            if (m_currentMove < m_playerUnit.Monster.Moves.Count - 1) m_currentMove++;
        }
        else if (m_inputReceiver.GetInputButton(BattleInputReceiver.Actions.LEFT, BattleInputReceiver.InputType.PRESSED))
        {
            if (m_currentMove > 0) m_currentMove--;
        }
        else if (m_inputReceiver.GetInputButton(BattleInputReceiver.Actions.UP, BattleInputReceiver.InputType.PRESSED))
        {
            if (m_currentMove > 1) m_currentMove -= 2;
        }
        else if (m_inputReceiver.GetInputButton(BattleInputReceiver.Actions.DOWN, BattleInputReceiver.InputType.PRESSED))
        {
            if (m_currentMove < m_playerUnit.Monster.Moves.Count - 2) m_currentMove += 2;
        }


        // 選択中の技の文字の色を変える
        m_dialogBox.UpdateMoveSelection(m_currentMove, m_playerUnit.Monster.Moves[m_currentMove]);
    }


    /// <summary>
    /// ・メッセージが出て、１秒後にアクションの選択を表示する
    /// ・Zボタンを押すと、MoveSelectorとMoveDetailsを表示する
    /// </summary>


}
