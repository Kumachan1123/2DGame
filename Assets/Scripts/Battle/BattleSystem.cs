using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit m_playerUnit;
    [SerializeField] BattleUnit m_enemyUnit;
    [SerializeField] BattleHud m_playerHud;
    [SerializeField] BattleHud m_enemyHud;
    [SerializeField] BattleDialogBox m_dialogBox;
    void Start()
    {
        m_playerUnit.SetUp();// プレイヤーのモンスターをセットアップ
        m_enemyUnit.SetUp();// 敵のモンスターをセットアップ

        m_playerHud.SetData(m_playerUnit.Monster);// プレイヤーのHUDをセット
        m_enemyHud.SetData(m_enemyUnit.Monster);// 敵のHUDをセット
        // ダイアログを表示
        StartCoroutine(m_dialogBox.TypeDialog($"A wild {m_enemyUnit.Monster.Base.Name} apeard."));

    }

}
