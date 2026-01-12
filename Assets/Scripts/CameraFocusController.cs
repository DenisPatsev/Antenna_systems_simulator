using System.Collections;
using UnityEngine;

public class CameraFocusController : MonoBehaviour
{
    public Camera camera;
    public PlayerInteractor playerInteractor;
    public PlayerController playerController;

    private CameraFocusableObject _cameraFocusableObject;

    private void OnEnable()
    {
        playerInteractor.OnFocus += InitializeFocusableObject;
    }

    private void InitializeFocusableObject(CameraFocusableObject focusableObject)
    {
        Unsubscribe();

        _cameraFocusableObject = focusableObject;

        _cameraFocusableObject.OnFocusStart += MoveCameraToObject;
        _cameraFocusableObject.OnFocusEnd += SetDefaultCameraPosition;
        
        _cameraFocusableObject.FocusOn();
    }

    private void MoveCameraToObject()
    {
        playerController.enabled = false;
        camera.transform.position = _cameraFocusableObject.cameraFocusPositionPoint.position;
        camera.transform.rotation = _cameraFocusableObject.cameraFocusPositionPoint.rotation;
    }

    private void SetDefaultCameraPosition()
    {
        camera.transform.localPosition = Constants.CameraDefaultPosition;
        playerController.enabled = true;
        // Cursor.visible = false;
    }

    private void Unsubscribe()
    {
        if (_cameraFocusableObject != null)
        {
            _cameraFocusableObject.OnFocusStart -= MoveCameraToObject;
            _cameraFocusableObject.OnFocusEnd -= SetDefaultCameraPosition;
        }
    }

    private void OnDisable()
    {
        Unsubscribe();
    }
}