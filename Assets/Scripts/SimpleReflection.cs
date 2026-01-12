using System;
using UnityEngine;

public class SimpleReflection : MonoBehaviour
{
    public Camera reflectionCamera;
    public RenderTexture renderTexture;
    public Vector3 rotationOffset;
    public Resolution reflectionResolution = Resolution.VeryLow128;

    private Camera _mainCamera;
    private Vector2 _resolution;

    public enum Resolution
    {
        VeryLow128 = 128,
        Low256 = 256,
        Medium512 = 512,
        High1024 = 1024,
    }

    private void OnEnable()
    {
        InitMainCamera();
    }

    private void OnValidate()
    {
        ChangeResolution();
    }

    private void LateUpdate()
    {
        if (_mainCamera == null)
        {
            InitMainCamera();
            return;
        }

        reflectionCamera.transform.position = new Vector3(_mainCamera.transform.position.x,
            -_mainCamera.transform.position.y + transform.position.y, _mainCamera.transform.position.z);
        reflectionCamera.transform.localRotation = Quaternion.Euler(
            -_mainCamera.transform.eulerAngles.x + rotationOffset.x,
            rotationOffset.y, rotationOffset.z);
    }

    private void InitMainCamera()
    {
        _mainCamera = Camera.main;

        if (_mainCamera != null)
        {
            reflectionCamera.transform.parent = _mainCamera.transform.parent;

            ChangeResolution();
        }
    }

    private void ChangeResolution()
    {
        if(_mainCamera == null)
            return;
        
        renderTexture.Release();
        _resolution = new Vector2(_mainCamera.pixelWidth, _mainCamera.pixelHeight);
        int resolution = 128;

        switch (reflectionResolution)
        {
            case Resolution.VeryLow128:
            {
                resolution = 128;
                break;
            }
            case Resolution.Low256:
            {
                resolution = 256;
                break;
            }
            case Resolution.Medium512:
            {
                resolution = 512;
                break;
            }
            case Resolution.High1024:
            {
                resolution = 1024;
                break;
            }
        }
        

        renderTexture.width = Mathf.RoundToInt(_resolution.x) * resolution / Mathf.RoundToInt(_resolution.y);
        renderTexture.height = resolution;
    }
}