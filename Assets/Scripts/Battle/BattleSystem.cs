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
    // PlayerMoveの実行
    IEnumerator PerformPlayerMove()
    {
        m_state = BattleState.BUSY;
        // 技を決定
        Move move = m_playerUnit.Monster.Moves[m_currentMove];
        // メッセージを表示
        yield return m_dialogBox.TypeDialog($"{m_playerUnit.Monster.Base.Name} は{move.Base.Name}をつかった");
        yield return new WaitForSeconds(1);
        // ダメージ計算
        bool isFainted = m_enemyUnit.Monster.TakeDamage(move, m_playerUnit.Monster);
        // HPバーを更新
        yield return m_enemyHud.UpdateHP();
        // 戦闘不能ならメッセージ
        if (isFainted)
        {
            yield return m_dialogBox.TypeDialog($"{m_enemyUnit.Monster.Base.Name} はたおれた！");
        }
        else
        {     // 戦闘可能ならEnemyMove();
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        m_state = BattleState.ENEMYMOVE;
        // 適当な技を選ぶ
        Move move = m_enemyUnit.Monster.GetRandomMove();
        // メッセージを表示
        yield return m_dialogBox.TypeDialog($"{m_enemyUnit.Monster.Base.Name} は{move.Base.Name}をつかった");
        yield return new WaitForSeconds(1);
        // ダメージ計算
        bool isFainted = m_playerUnit.Monster.TakeDamage(move, m_enemyUnit.Monster);
        // HPバーを更新
        yield return m_playerHud.UpdateHP();
        // 戦闘不能ならメッセージ
        if (isFainted)
        {
            yield return m_dialogBox.TypeDialog($"{m_playerUnit.Monster.Base.Name} はたおれた！");
        }
        else
        {     // 戦闘可能ならPlayerAction();
            PlayerAction();
        }
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

        if (m_inputReceiver.GetInputButton(BattleInputReceiver.Actions.SELECT, BattleInputReceiver.InputType.PRESSED))
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
        if (m_inputReceiver.GetInputButton(BattleInputReceiver.Actions.SELECT, BattleInputReceiver.InputType.PRESSED))
        {
            // 技が選ばれたときの処理
            Debug.Log($"{m_playerUnit.Monster.Moves[m_currentMove].Base.Name} が選ばれた");
            // 技選択のUIは非表示にする
            m_dialogBox.EnableMoveSelector(false);
            // メッセージ復活
            m_dialogBox.EnableDialogText(true);
            // 技の実行
            StartCoroutine(PerformPlayerMove());
        }
    }


    /// <summary>
    /// ・メッセージが出て、１秒後にアクションの選択を表示する
    /// ・Zボタンを押すと、MoveSelectorとMoveDetailsを表示する
    /// </summary>


}
