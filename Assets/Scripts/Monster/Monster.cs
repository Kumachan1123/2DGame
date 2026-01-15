// レベルに応じたステータスの違うモンスターを生成するクラス
// データのみを扱う純粋C#のクラス
using UnityEngine;
using System.Collections.Generic;

public class Monster
{
    // ベースとなるデータ
    public MonsterBase Base { get; set; }
    // レベル
    public int Level { get; set; }

    public int HP { get; set; }
    // 使える技
    public List<Move> Moves { get; set; }
    // コンストラクタ
    public Monster(MonsterBase pBase, int pLevel)
    {
        Base = pBase;
        Level = pLevel;
        // 現在のHPを最大HPに設定
        HP = MaxHP;
        Moves = new List<Move>();
        // 覚える技の設定：覚える技のレベル以上なら、Movesに追加
        foreach (LearnableMove move in Base.LearnableMoves)
        {
            if (Level >= move.Level)
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
    public int MaxHP { get { return Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10; } }
    // 攻撃
    public int Attack { get { return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5; } }
    // 防御
    public int Defense { get { return Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5; } }
    // 特攻
    public int SpecialAttack { get { return Mathf.FloorToInt((Base.SpecialAttack * Level) / 100f) + 5; } }
    // 特防
    public int SpecialDefense { get { return Mathf.FloorToInt((Base.SpecialDefense * Level) / 100f) + 5; } }
    // 素早さ
    public int Speed { get { return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5; } }
    // ダメージ
    // ・戦闘不能
    // ・クリティカル
    // ・タイプ相性
    // を渡す
    public DamageDetails TakeDamage(Move move, Monster attacker)
    {
        // クリティカル
        float critical = 1f;
        // 6.25%の確率でクリティカル
        if (Random.value * 100f <= 6.25f)
        {
            critical = 2f;
        }
        // 相性
        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) *
                     TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);
        DamageDetails damageDetails = new DamageDetails()
        {
            Fainted = false,
            Critical = critical,
            TypeEffectiveness = type
        };

        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);
        Debug.Log($"{damage} のダメージ！{HP}-{damage}={HP - damage}");
        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}


public class DamageDetails
{
    // 戦闘不能
    public bool Fainted { get; set; }
    // クリティカル
    public float Critical { get; set; }
    // タイプ相性
    public float TypeEffectiveness { get; set; }
}