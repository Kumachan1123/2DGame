using DG.Tweening;

public interface IFadeStrategy
{
    void FadeIn(UIFade context);
    void FadeOut(UIFade context);
    Tween GetFadeOutTween(UIFade context);
    Tween GetFadeInTween(UIFade context);
}