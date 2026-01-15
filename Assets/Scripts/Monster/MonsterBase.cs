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

public class TypeChart
{
    static float[][] chart =
    {
        // 攻撃\防御       NOR FIRE WATER GRASS ELECTRIC ICE FIGHTING POISON GROUND FLYING PSYCHIC BUG ROCK GHOST DRAGON DARK STEEL FAIRY
        /*NOR*/new float[]{1f, 1f,  1f,    1f,     1f,    1f,    1f,     1f,    1f,    1f,    1f,  1f, 0.5f,  0f,    1f,  1f,  0.5f,  1f },
        /*FIR*/new float[]{1f, 0.5f,0.5f,  2f,     1f,    2f,    1f,     1f,    1f,    1f,    1f,  2f, 0.5f,  1f,    0.5f,1f,  2f,    1f },
        /*WAT*/new float[]{1f, 2f,  0.5f,  0.5f,   1f,    1f,    1f,     1f,    2f,    1f,    1f,  1f, 2f,    1f,    0.5f,1f,  1f,    1f },
        /*GRA*/new float[]{1f, 0.5f,2f,    0.5f,   1f,    1f,    1f,     0.5f,  2f,    0.5f,  1f,  0.5f,2f,    1f,    0.5f,1f,  0.5f,  1f },
        /*ELE*/new float[]{1f, 1f,  2f,    0.5f,   0.5f,  1f,    1f,     1f,    0f,    2f,    1f,  1f, 1f,    1f,    0.5f,1f,  1f,    1f },
        /*ICE*/new float[]{1f, 0.5f,0.5f,  2f,     1f,    0.5f,  1f,     1f,    2f,    2f,    1f,  1f, 1f,    1f,    2f,  1f,  0.5f,  1f },
        /*FIG*/new float[]{2f, 1f,  1f,    1f,     1f,    2f,    1f,     0.5f,  1f,    0.5f,  0.5f,2f, 2f,    0f,    1f,  2f,  2f,    0.5f},
        /*POI*/new float[]{1f, 1f,  1f,    2f,     1f,    1f,    1f,     0.5f,  0.5f,  1f,    1f,  1f, 0.5f,  0.5f,  1f,  1f,  0f,    2f },
        /*GRO*/new float[]{1f, 2f,  1f,    0.5f,   2f,    1f,    1f,     2f,    1f,    0f,    1f,  0.5f,2f,    1f,    1f,  1f,  2f,    1f },
        /*FLY*/new float[]{1f, 1f,  1f,    2f,     0.5f,  1f,    2f,     1f,    1f,    1f,    1f,  2f, 0.5f,  1f,    1f,  1f,  0.5f,  1f },
        /*PSY*/new float[]{1f, 1f,  1f,    1f,     1f,    1f,    2f,     2f,    1f,    1f,    0.5f,1f, 1f,    1f,    1f,  0f,  0.5f,  1f },
        /*BUG*/new float[]{1f, 0.5f,1f,    2f,     1f,    1f,    0.5f,   0.5f,  1f,    0.5f,  2f,  1f, 1f,    0.5f,  1f,  2f,  0.5f,  0.5f},
        /*ROC*/new float[]{1f, 2f,  1f,    1f,     1f,    2f,    0.5f,   1f,    0.5f,  2f,    1f,  2f, 1f,    1f,    1f,  1f,  0.5f,  1f },
        /*GHO*/new float[]{0f, 1f,  1f,    1f,     1f,    1f,    1f,     1f,    1f,    1f,    2f,  1f, 1f,    2f,    1f,  0.5f,  1f,  1f } ,
        /*DRA*/new float[]{1f, 1f,  1f,    1f,     1f,    1f,    1f,     1f,    1f,    1f,    1f,  1f, 1f,    1f,    2f,  1f,  0.5f,  0f },
        /*DAR*/new float[]{1f, 1f,  1f,    1f,     1f,    1f,    0.5f,   1f,    1f,    1f,    2f,  1f, 1f,    2f,    1f,  0.5f,  1f,  0.5f},
        /*STE*/new float[]{1f, 0.5f,0.5f,  1f,     1f,    2f,    1f,     1f,    1f,    1f,    1f,  1f, 2f,    1f,    1f,  1f,  0.5f,  2f },
        /*FAI*/new float[]{1f, 0.5f,1f,    1f,     1f,    1f,    2f,     0.5f,  1f,    1f,    1f,  1f, 1f,    1f,    2f,  2f,  0.5f,  1f }
               };
    public static float GetEffectiveness(MonsterType attackType, MonsterType defenseType)
    {
        if (attackType == MonsterType.None || defenseType == MonsterType.None)
            return 1f;
        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;
        return chart[row][col];
    }

}