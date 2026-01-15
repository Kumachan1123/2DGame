// 
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleHud : MonoBehaviour
{
    [SerializeField, Header("名前")] Text m_nameText;
    [SerializeField, Header("レベル")] Text m_levelText;
    [SerializeField, Header("HPバー")] HPBar m_hpBar;
    public void SetData(Monster monster)
    {
        m_nameText.text = monster.Base.Name;
        m_levelText.text = "Lv " + monster.Level;
        m_hpBar.SetHP((float)(monster.HP / monster.MaxHP));
    }
}
