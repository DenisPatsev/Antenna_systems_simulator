using System;
using UnityEngine;

public class LightSettings : MonoBehaviour
{
    [SerializeField] private Light[] _laboratoryLights;
    [SerializeField] private Light[] _bezechCameraLights;
    [SerializeField] private Material _fakeLightMaterial;

    [ColorUsage(true, true)] [SerializeField]
    private Color _laboratoryLightColor;

    [ColorUsage(true, true)] [SerializeField]
    private Color _bezechCameraLightColor;
    
    [ColorUsage(true, true)] [SerializeField]
    private Color _fakeLightColor;

    [Range(0, 5)] public float laboratoryLightIntensity;
    [Range(0, 5)] public float bezechCameraLightIntensity;
    
    private void Start()
    {
        ApplySettings();
    }

    private void ApplySettings()
    {
        foreach (var light in _laboratoryLights)
        {
            if (light != null)
            {
                light.intensity = laboratoryLightIntensity;
                light.color = _laboratoryLightColor;
            }
        }

        foreach (var light in _bezechCameraLights)
        {
            if (light != null)
            {
                light.intensity = bezechCameraLightIntensity;
                light.color = _bezechCameraLightColor;
            }
        }
        
        _fakeLightMaterial.SetColor("_GlowColor", _fakeLightColor);
    }

    private void OnValidate()
    {
        ApplySettings();
    }
}