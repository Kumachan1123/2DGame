using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class UIFade : MonoBehaviour
{
    public enum FadeType { Move, AlphaChange, Both }

    [Header("フェードタイプ")]
    [SerializeField] public FadeType _fadeType = FadeType.Move;

    [Header("完全表示状態の座標（フェードイン完了時）")]
    [SerializeField] public Vector3 _fadeInPosition = Vector3.zero;
    [Header("完全非表示状態の座標（フェードアウト完了時）")]
    [SerializeField] public Vector3 _fadeOutPosition = Vector3.zero;

    [Header("完全表示状態のアルファ（フェードイン完了時）")]
    [SerializeField] public float _fadeInAlpha = 1f;
    [Header("完全非表示状態のアルファ（フェードアウト完了時）")]
    [SerializeField] public float _fadeOutAlpha = 0f;

    [Header("フェード速度")]
    [SerializeField] public float _fadeSpeed = 1f;
    [Header("イージングの種類")]
    [SerializeField] public Ease _easeType = Ease.Linear;

    [Header("フェード対象Image（AlphaChange用）")]
    [SerializeField] public Graphic _targetGraphic;

    [Header("開始時に表示状態で始めるか？（true=表示/false=非表示）")]
    [SerializeField] public bool _isStartVisible = true;

    private RectTransform _rect;
    public RectTransform Rect
    {
        get
        {
            if (_rect == null) _rect = GetComponent<RectTransform>();
            return _rect;
        }
    }

    private IFadeStrategy _fadeStrategy;

    private void Awake()
    {
        _fadeStrategy = CreateStrategy(_fadeType);

        // 非表示状態で開始
        Rect.localPosition = _fadeOutPosition;
        if (_targetGraphic != null)
        {
            var c = _targetGraphic.color;
            c.a = _fadeOutAlpha;
            _targetGraphic.color = c;
        }

        // 起動時にフェードイン
        FadeInWithCallback(() => { });
    }

    public void FadeIn()
    {
        _fadeStrategy.FadeIn(this);
    }

    public void FadeOut()
    {
        _fadeStrategy.FadeOut(this);
    }

    private IFadeStrategy CreateStrategy(FadeType type)
    {
        switch (type)
        {
            case FadeType.Move: return new MoveFadeStrategy();
            case FadeType.AlphaChange: return new AlphaFadeStrategy();
            case FadeType.Both: return new BothFadeStrategy();
            default: return new MoveFadeStrategy();
        }
    }

    public void FadeInWithCallback(Action onComplete)
    {
        Tween tween = _fadeStrategy.GetFadeInTween(this);
        if (tween != null)
            tween.OnComplete(() => onComplete?.Invoke());
    }

    public void FadeOutWithCallback(Action onComplete)
    {
        Tween tween = _fadeStrategy.GetFadeOutTween(this);
        if (tween != null)
            tween.OnComplete(() => onComplete?.Invoke());
    }
}