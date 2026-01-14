using UnityEngine;
[CreateAssetMenu(fileName = "MoveBase", menuName = "Monster/MoveBase")]
public class MoveBase : ScriptableObject
{
    // 技のマスターデータ
    // 技の名前、説明、タイプ、威力、命中率、PP
    [SerializeField] new string m_name;
    [SerializeField] string m_description;
    [SerializeField] MonsterType m_type;
    [SerializeField] int m_power;
    [SerializeField] int m_accuracy;
    [SerializeField] int m_maxPP;

    public string Name { get => m_name; }
    public string Description { get => m_description; }
    public MonsterType Type { get => m_type; }
    public int Power { get => m_power; }
    public int Accuracy { get => m_accuracy; }
    public int MaxPP { get => m_maxPP; }

}
