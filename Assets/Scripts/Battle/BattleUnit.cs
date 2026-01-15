using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] MonsterBase m_base;
    [SerializeField] int m_level;
    [SerializeField] bool m_isPlayerUnit;
    public Monster Monster { get; set; }

    // バトルで使うモンスターを保持
    // モンスターの画像を反映する
    public void SetUp()
    {
        // baseからレベルに応じたモンスターを生成する
        // BattleSystemで使うからプロパティに入れる
        Monster = new Monster(m_base, m_level);
        Image image = GetComponent<Image>();
        if (m_isPlayerUnit) image.sprite = Monster.Base.BackSprite;
        else image.sprite = Monster.Base.FrontSprite;
    }
}
