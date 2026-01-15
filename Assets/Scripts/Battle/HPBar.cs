// HPバーを表示・更新するクラス
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    // HPの増減を描画する
    [SerializeField] GameObject m_health;

    public void SetHP(float hp)
    {
        m_health.transform.localScale = new Vector3(hp, 1, 1);
    }
}
