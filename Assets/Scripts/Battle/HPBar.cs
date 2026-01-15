// HPバーを表示・更新するクラス
using System;
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

    public IEnumerator SetHPSmooth(float newHP)
    {
        // 現在のHPを取得
        float currentHP = m_health.transform.localScale.x;
        // HPの変化量
        float changeAmount = currentHP - newHP;
        // HPが変化するまでループ
        while (currentHP - newHP > Mathf.Epsilon)
        {
            // 現在のHPを徐々に新しいHPに近づける
            currentHP -= changeAmount * Time.deltaTime;
            // －にならないように調整
            if (currentHP < 0f) currentHP *= 0f;
            // HPバーのスケールを更新
            m_health.transform.localScale = new Vector3(currentHP, 1, 1);
            // 次のフレームまで待機
            yield return null;
        }



        // HPバーのスケールを更新
        m_health.transform.localScale = new Vector3(currentHP, 1, 1);
    }
}
