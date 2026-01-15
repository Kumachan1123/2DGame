using DG.Tweening;
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
    Vector3 m_originalPos;
    Color m_originalColor;
    Image m_image;

    private void OnEnable()
    {
        // 初期位置に配置
        PlayIdlePositioning();
        // 色を元に戻す
        m_image.color = m_originalColor;
    }

    private void Awake()
    {
        m_image = GetComponent<Image>();
        m_originalPos = transform.localPosition;
        m_originalColor = m_image.color;
    }

    // バトルで使うモンスターを保持
    // モンスターの画像を反映する
    public void SetUp()
    {

        // baseからレベルに応じたモンスターを生成する
        // BattleSystemで使うからプロパティに入れる
        Monster = new Monster(m_base, m_level);
        if (m_isPlayerUnit) m_image.sprite = Monster.Base.BackSprite;
        else m_image.sprite = Monster.Base.FrontSprite;

        // 登場演出
        PlayerEnterAnimation();

    }
    // 初期位置に配置
    public void PlayIdlePositioning()
    {
        // 左端に配置
        if (m_isPlayerUnit)
            transform.localPosition = new Vector3(-1200f, m_originalPos.y, 0f);
        // 右端に配置
        else
            transform.localPosition = new Vector3(1200f, m_originalPos.y, 0f);
    }

    // 登場演出
    public void PlayerEnterAnimation()
    {

        // 戦闘時の位置までアニメーション
        transform.DOLocalMoveX(m_originalPos.x, .5f);
    }
    // 攻撃演出
    public void PlayerAttackAnimation()
    {
        // シーケンス
        // 右に動いた後、元の位置に戻る
        Sequence sequence = DOTween.Sequence();
        // 右に50移動
        sequence.Append(transform.DOLocalMoveX(m_originalPos.x + 50f, .3f));
        // 元の位置に戻る
        sequence.Append(transform.DOLocalMoveX(m_originalPos.x, .3f));
    }
    // ダメージ演出
    public void PlayerHitAnimation()
    {

        // 色をグレーにする
        Sequence sequence = DOTween.Sequence();
        // グレーに変化
        sequence.Append(m_image.DOColor(Color.gray, .1f));
        // 元の色に戻す
        sequence.Append(m_image.DOColor(m_originalColor, .1f));
    }
    // 戦闘不能演出
    public void PlayerFaintAnimation()
    {
        // 下に落ちる＆透明化
        Sequence sequence = DOTween.Sequence();
        // 下に移動
        sequence.Append(transform.DOLocalMoveY(m_originalPos.y - 150f, .5f));
        // 透明化
        sequence.Join(m_image.DOFade(0f, .5f));
    }
}
