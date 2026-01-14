using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIFade))]
public class FadeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        UIFade fade = (UIFade)target;

        // フェードタイプの選択
        fade._fadeType = (UIFade.FadeType)EditorGUILayout.EnumPopup("フェードタイプ", fade._fadeType);

        if (fade._fadeType == UIFade.FadeType.Move || fade._fadeType == UIFade.FadeType.Both)
        {
            EditorGUILayout.LabelField("完全にフェードインした場合の座標（表示状態）");
            fade._fadeInPosition = EditorGUILayout.Vector3Field("Fade In Position", fade._fadeInPosition);

            EditorGUILayout.LabelField("完全にフェードアウトした場合の座標（非表示状態）");
            fade._fadeOutPosition = EditorGUILayout.Vector3Field("Fade Out Position", fade._fadeOutPosition);
        }
        if (fade._fadeType == UIFade.FadeType.AlphaChange || fade._fadeType == UIFade.FadeType.Both)
        {
            EditorGUILayout.LabelField("完全にフェードインした場合のアルファ（表示状態）");
            fade._fadeInAlpha = EditorGUILayout.FloatField("Fade In Alpha", fade._fadeInAlpha);

            EditorGUILayout.LabelField("完全にフェードアウトした場合のアルファ（非表示状態）");
            fade._fadeOutAlpha = EditorGUILayout.FloatField("Fade Out Alpha", fade._fadeOutAlpha);

            fade._targetGraphic = (UnityEngine.UI.Graphic)EditorGUILayout.ObjectField("フェード対象Graphic", fade._targetGraphic, typeof(UnityEngine.UI.Graphic), true);
        }

        fade._fadeSpeed = EditorGUILayout.FloatField("フェード速度", fade._fadeSpeed);
        fade._easeType = (DG.Tweening.Ease)EditorGUILayout.EnumPopup("イージングの種類", fade._easeType);
        fade._isStartVisible = EditorGUILayout.Toggle("開始時に表示状態で始めるか？", fade._isStartVisible);
        if (GUI.changed)
        {
            EditorUtility.SetDirty(fade);
        }
    }
}