using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MonsterData", menuName = "Monster/MonsterDatas")]
// モンスターのマスターデータ：インスペクターでだけ編集可能
public class MonsterBase : ScriptableObject
{
    // モンスターの名前、説明、属性、ステータス
    [SerializeField] new string m_name;
    [SerializeField] string m_description;
    // タイプ
    [SerializeField] Sprite m_frontSprite;
    [SerializeField] Sprite m_backSprite;
    // 属性1
    [SerializeField] MonsterType m_type1;
    // 属性2
    [SerializeField] MonsterType m_type2;
    // ステータス
    [SerializeField] int m_maxHP;
    [SerializeField] int m_attack;
    [SerializeField] int m_defense;
    [SerializeField] int m_specialAttack;
    [SerializeField] int m_specialDefense;
    [SerializeField] int m_speed;

    // 覚える技のリスト
    [SerializeField] List<LearnableMove> m_learnableMoves;


    public string Name { get { return m_name; } }
    public string Description { get { return m_description; } }
    public Sprite FrontSprite { get { return m_frontSprite; } }
    public Sprite BackSprite { get { return m_backSprite; } }
    public MonsterType Type1 { get { return m_type1; } }
    public MonsterType Type2 { get { return m_type2; } }
    public int MaxHP { get { return m_maxHP; } }
    public int Attack { get { return m_attack; } }
    public int Defense { get { return m_defense; } }
    public int SpecialAttack { get { return m_specialAttack; } }
    public int SpecialDefense { get { return m_specialDefense; } }
    public int Speed { get { return m_speed; } }
    public List<LearnableMove> LearnableMoves { get { return m_learnableMoves; } }
}
[System.Serializable]
// モンスターが覚える技:どのレベルで覚えるか
public class LearnableMove
{
    // ヒエラルキーで設定
    [SerializeField] MoveBase m_moveBase;
    [SerializeField] int m_level;

    public MoveBase Base { get { return m_moveBase; } }
    public int Level { get { return m_level; } }
}

// 属性
public enum MonsterType
{
    None = 0,   // 無し
    Normal,     // ノーマル
    Fire,       // 炎
    Water,      // 水
    Electric,   // 電気
    Grass,      // 草
    Ice,        // 氷
    Fighting,   // 格闘
    Poison,     // 毒
    Ground,     // 地面
    Flying,     // 飛行
    Psychic,    // エスパー
    Dark,       // 悪
    Bug,        // 虫
    Rock,       // 岩
    Ghost,      // ゴースト
    Dragon,     // ドラゴン
    Steel,      // 鋼
    Fairy       // フェアリー
}