// レベルに応じたステータスの違うモンスターを生成するクラス
// データのみを扱う純粋C#のクラス
using UnityEngine;
using System.Collections.Generic;

public class Monster
{
    // ベースとなるデータ
    [SerializeField] MonsterBase m_base;
    // レベル
    int m_level;

    public int HP { get; set; }
    // 使える技
    public List<Move> Moves { get; set; }
    // コンストラクタ
    public Monster(MonsterBase pBase, int pLevel)
    {
        m_base = pBase;
        m_level = pLevel;
        // 現在のHPを最大HPに設定
        HP = pBase.MaxHP;
        // 覚える技の設定：覚える技のレベル以上なら、Movesに追加
        foreach (LearnableMove move in m_base.LearnableMoves)
        {
            if (m_level >= move.Level)
            {
                Moves.Add(new Move(move.Base));
            }
            // 最大4つまで
            if (Moves.Count >= 4)
            {
                break;
            }
        }
    }
    // レベルに応じたステータスを返すもの
    // HP
    public int MaxHP { get { return Mathf.FloorToInt((m_base.MaxHP * m_level) / 100f) + 10; } }
    // 攻撃
    public int Attack { get { return Mathf.FloorToInt((m_base.Attack * m_level) / 100f) + 5; } }
    // 防御
    public int Defense { get { return Mathf.FloorToInt((m_base.Defense * m_level) / 100f) + 5; } }
    // 特攻
    public int SpecialAttack { get { return Mathf.FloorToInt((m_base.SpecialAttack * m_level) / 100f) + 5; } }
    // 特防
    public int SpecialDefense { get { return Mathf.FloorToInt((m_base.SpecialDefense * m_level) / 100f) + 5; } }
    // 素早さ
    public int Speed { get { return Mathf.FloorToInt((m_base.Speed * m_level) / 100f) + 5; } }



}
