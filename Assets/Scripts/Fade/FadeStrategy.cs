using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MoveFadeStrategy : IFadeStrategy
{
    public void FadeIn(UIFade context)
    {
        // 非表示位置から表示位置へ
        context.Rect.localPosition = context._fadeOutPosition;
        context.Rect.DOLocalMove(context._fadeInPosition, context._fadeSpeed).SetEase(context._easeType);
    }
    public void FadeOut(UIFade context)
    {
        // 表示位置から非表示位置へ
        context.Rect.localPosition = context._fadeInPosition;
        context.Rect.DOLocalMove(context._fadeOutPosition, context._fadeSpeed).SetEase(context._easeType);
    }
    public Tween GetFadeInTween(UIFade context)
    {
        context.Rect.localPosition = context._fadeOutPosition;
        return context.Rect.DOLocalMove(context._fadeInPosition, context._fadeSpeed).SetEase(context._easeType);
    }
    public Tween GetFadeOutTween(UIFade context)
    {
        context.Rect.localPosition = context._fadeInPosition;
        return context.Rect.DOLocalMove(context._fadeOutPosition, context._fadeSpeed).SetEase(context._easeType);
    }
}
public class AlphaFadeStrategy : IFadeStrategy
{
    public void FadeIn(UIFade context)
    {
        if (context._targetGraphic != null)
        {
            var c = context._targetGraphic.color;
            c.a = context._fadeOutAlpha;
            context._targetGraphic.color = c;
            context._targetGraphic.DOFade(context._fadeInAlpha, context._fadeSpeed).SetEase(context._easeType);
        }
    }
    public void FadeOut(UIFade context)
    {
        if (context._targetGraphic != null)
        {
            var c = context._targetGraphic.color;
            c.a = context._fadeInAlpha;
            context._targetGraphic.color = c;
            context._targetGraphic.DOFade(context._fadeOutAlpha, context._fadeSpeed).SetEase(context._easeType);
        }
    }
    public Tween GetFadeInTween(UIFade context)
    {
        if (context._targetGraphic != null)
        {
            var c = context._targetGraphic.color;
            c.a = context._fadeOutAlpha;
            context._targetGraphic.color = c;
            return context._targetGraphic.DOFade(context._fadeInAlpha, context._fadeSpeed).SetEase(context._easeType);
        }
        return null;
    }
    public Tween GetFadeOutTween(UIFade context)
    {
        if (context._targetGraphic != null)
        {
            var c = context._targetGraphic.color;
            c.a = context._fadeInAlpha;
            context._targetGraphic.color = c;
            return context._targetGraphic.DOFade(context._fadeOutAlpha, context._fadeSpeed).SetEase(context._easeType);
        }
        return null;
    }
}
public class BothFadeStrategy : IFadeStrategy
{
    private MoveFadeStrategy move = new MoveFadeStrategy();
    private AlphaFadeStrategy alpha = new AlphaFadeStrategy();
    public void FadeIn(UIFade context)
    {
        move.FadeIn(context);
        alpha.FadeIn(context);
    }
    public void FadeOut(UIFade context)
    {
        move.FadeOut(context);
        alpha.FadeOut(context);
    }
    public Tween GetFadeInTween(UIFade context)
    {
        var seq = DOTween.Sequence();
        var moveTween = move.GetFadeInTween(context);
        var alphaTween = alpha.GetFadeInTween(context);
        if (moveTween != null) seq.Join(moveTween);
        if (alphaTween != null) seq.Join(alphaTween);
        return seq;
    }
    public Tween GetFadeOutTween(UIFade context)
    {
        var seq = DOTween.Sequence();
        var moveTween = move.GetFadeOutTween(context);
        var alphaTween = alpha.GetFadeOutTween(context);
        if (moveTween != null) seq.Join(moveTween);
        if (alphaTween != null) seq.Join(alphaTween);
        return seq;
    }
}