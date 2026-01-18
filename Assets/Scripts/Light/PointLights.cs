//      PointLights.cs
//      このスクリプトを持つオブジェクトの子オブジェクトに配置されたポイントライトを管理するスクリプト

using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

[ExecuteAlways]
public class PointLights : MonoBehaviour
{
    // ポイントライトのリスト
    private List<Light2D> m_pointLights = new List<Light2D>();

    // ライトの強度
    [SerializeField, Header("ライトの強度")]
    private float m_lightIntensity = 1f;

    // 光の色
    [SerializeField, Header("光の色")]
    private Color m_lightColor = Color.white;

    // 太陽
    [SerializeField, Header("太陽")]
    private Sun m_sun;

    private void Awake()
    {
        CollectPointLights();
        ApplyLightSettings();
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        UpdateLightOnOffByTime();
    }

    /// <summary>
    /// インスペクターの値が変更されたときに呼ばれる
    /// </summary>
    private void OnValidate()
    {
        CollectPointLights();
        ApplyLightSettings();
    }

    /// <summary>
    /// 子オブジェクトからポイントライトを収集する
    /// </summary>
    private void CollectPointLights()
    {
        m_pointLights.Clear();

        Light2D[] lights = GetComponentsInChildren<Light2D>();
        foreach (Light2D light in lights)
        {
            if (light.lightType == Light2D.LightType.Point)
            {
                m_pointLights.Add(light);
            }
        }
    }

    /// <summary>
    /// ポイントライトの色と強度を適用する
    /// </summary>
    private void ApplyLightSettings()
    {
        foreach (Light2D pointLight in m_pointLights)
        {
            pointLight.intensity = m_lightIntensity;
            pointLight.color = m_lightColor;
        }
    }

    /// <summary>
    /// ゲーム内時間に応じてライトの点灯状態を更新する
    /// </summary>
    private void UpdateLightOnOffByTime()
    {
        if (m_sun == null)
        {
            return;
        }

        float timeOfDay = m_sun.TimeOfDay;

        bool isNight = !(timeOfDay > 6f && timeOfDay <= 18f);

        foreach (Light2D pointLight in m_pointLights)
        {
            pointLight.enabled = isNight;
        }
    }
}
