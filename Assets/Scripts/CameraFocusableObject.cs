using System;
using UnityEngine;
using UnityEngine.Events;

public class CameraFocusableObject : MonoBehaviour, ICameraFocusable
{
    public Transform cameraFocusPositionPoint;
    public UnityAction OnFocusStart;
    public UnityAction OnFocusEnd;

    private PlayerInteractor _playerInteractor;

    public void FocusOn()
    {
        OnFocusStart?.Invoke();
        CursorStateChanger.EnableCursor();

        if (_playerInteractor != null)
            _playerInteractor.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            FocusOff();
        }
    }

    public void FocusOff()
    {
        OnFocusEnd?.Invoke();
        CursorStateChanger.DisableCursor();

        if (_playerInteractor != null)
            _playerInteractor.enabled = true;
    }

    public void InitializeInteractor(PlayerInteractor interactor)
    {
        _playerInteractor = interactor;
    }
}