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
    // フェード管理クラス
    [SerializeField, Header("フェード")]
    private UIFade m_fade;
    // プレイヤー
    [SerializeField, Header("プレイヤー")]
    private GameObject m_player;
    public int m_currentAction = 0;// 現在のアクション 0:Fight, 1:Run
    int m_currentMove = 0;// 0:左上　1:右上　2:左下　3:右下
    BattleState m_state;
    // インプットレシーバー
    private BattleInputReceiver m_inputReceiver;

    // 有効化されたときに一度だけ呼ばれる
    private void OnEnable()
    {
        // アクションセレクターを有効化
        m_dialogBox.EnableActionSelector(false);
        m_playerHud.EnableBattleHud(false);
        m_enemyHud.EnableBattleHud(false);
        m_fade.FadeInWithCallback(() =>
        {

            // フィールド上のプレイヤーを非アクティブにする
            m_player.SetActive(false);
            // 戦うにカーソルが置かれた状態にする
            m_currentAction = 0;
            // ダイアログを表示
            StartCoroutine(SetUpBattle());
        });

    }

    void Start()
    {
        m_inputReceiver = GetComponent<BattleInputReceiver>();

    }

    IEnumerator SetUpBattle()
    {
        // バトルステートを設定
        m_state = BattleState.START;
        m_playerUnit.SetUp();// プレイヤーのモンスターをセットアップ
        m_enemyUnit.SetUp();// 敵のモンスターをセットアップ

        m_playerHud.EnableBattleHud(true);// プレイヤーのHUDを有効化
        m_enemyHud.EnableBattleHud(true);// 敵のHUDを有効化
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
    // PlayerMoveの実行(プレイヤーの技選択後)
    IEnumerator PerformPlayerMove()
    {
        m_state = BattleState.BUSY;
        // 技を決定
        Move move = m_playerUnit.Monster.Moves[m_currentMove];
        // メッセージを表示
        yield return m_dialogBox.TypeDialog($"{m_playerUnit.Monster.Base.Name} は{move.Base.Name}をつかった");
        yield return new WaitForSeconds(1);
        // 攻撃演出
        m_playerUnit.PlayerAttackAnimation();
        yield return new WaitForSeconds(0.7f);
        // 敵ヒット演出
        m_enemyUnit.PlayerHitAnimation();
        yield return new WaitForSeconds(0.2f);
        // ダメージ計算
        DamageDetails damageDetails = m_enemyUnit.Monster.TakeDamage(move, m_playerUnit.Monster);
        // HPバーを更新
        yield return m_enemyHud.UpdateHP();
        // 相性・クリティカルのメッセージ表示
        yield return ShowDamageDetails(damageDetails);
        // 戦闘不能ならメッセージ
        if (damageDetails.Fainted)
        {
            yield return m_dialogBox.TypeDialog($"{m_enemyUnit.Monster.Base.Name} はたおれた！");
            // 戦闘不能演出
            m_enemyUnit.PlayerFaintAnimation();
            // フィールドに戻る
            StartCoroutine(ExitBattleSystem(2f));
        }
        else
        {     // 戦闘可能ならEnemyMove();
            StartCoroutine(EnemyMove());
        }
    }
    // EnemyMoveの実行(敵の技選択後)
    IEnumerator EnemyMove()
    {
        m_state = BattleState.ENEMYMOVE;
        // 適当な技を選ぶ
        Move move = m_enemyUnit.Monster.GetRandomMove();
        // メッセージを表示
        yield return m_dialogBox.TypeDialog($"{m_enemyUnit.Monster.Base.Name} は{move.Base.Name}をつかった");
        yield return new WaitForSeconds(1);
        // 攻撃演出
        m_enemyUnit.PlayerAttackAnimation();
        yield return new WaitForSeconds(0.7f);
        // プレイヤーヒット演出
        m_playerUnit.PlayerHitAnimation();
        yield return new WaitForSeconds(0.2f);
        // ダメージ計算
        DamageDetails damageDetails = m_playerUnit.Monster.TakeDamage(move, m_enemyUnit.Monster);
        // HPバーを更新
        yield return m_playerHud.UpdateHP();
        // クリティカル・相性のメッセージ表示
        yield return ShowDamageDetails(damageDetails);
        // 戦闘不能ならメッセージ
        if (damageDetails.Fainted)
        {
            yield return m_dialogBox.TypeDialog($"{m_playerUnit.Monster.Base.Name} はたおれた！");
            // 戦闘不能演出
            m_playerUnit.PlayerFaintAnimation();
            // フィールドに戻る
            StartCoroutine(ExitBattleSystem(2f));
        }
        else
        {     // 戦闘可能ならPlayerAction();
            PlayerAction();
        }
    }
    // クリティカル・タイプ相性のメッセージ表示
    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        float critical = damageDetails.Critical;
        float typeEffectiveness = damageDetails.TypeEffectiveness;
        if (critical > 1f)
        {
            yield return m_dialogBox.TypeDialog("きゅうしょに あたった！");
            yield return new WaitForSeconds(1);
        }
        if (typeEffectiveness > 1f)
        {
            yield return m_dialogBox.TypeDialog("こうかは ばつぐんだ！");
            yield return new WaitForSeconds(1);
        }
        else if (typeEffectiveness < 1f && typeEffectiveness > 0f)
        {
            yield return m_dialogBox.TypeDialog("こうかは いまひとつのようだ...");
            yield return new WaitForSeconds(1);
        }
        else if (typeEffectiveness == 0f)
        {
            yield return m_dialogBox.TypeDialog("こうかは なかった...");
            yield return new WaitForSeconds(1);
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
            switch (m_currentAction)
            {
                case 0:
                    Debug.Log("Fightが選ばれた");
                    PlayerMove();
                    break;
                case 1:
                    Debug.Log("Runが選ばれた");
                    StartCoroutine(m_dialogBox.TypeDialog("逃げ出した！"));
                    StartCoroutine(ExitBattleSystem(2f));
                    break;
            }
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



    private IEnumerator ExitBattleSystem(float duration)
    {
        yield return new WaitForSeconds(duration);
        m_fade.FadeOutWithCallback(() =>
        {
            m_player.SetActive(true);
            m_player.GetComponent<PlayerController>().IsEncountered = false;
            // ダイアログテキストを初期化
            m_dialogBox.InitializeDialogText();
            // バトルシステムを無効化してフィールドに戻る
            this.gameObject.SetActive(false);
        });
    }

}
