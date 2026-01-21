using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraFocusController : MonoBehaviour
{
    public Camera camera;
    public PlayerInteractor playerInteractor;
    public PlayerController playerController;

    private CameraFocusableObject _cameraFocusableObject;
    private InteractableObject _interactableObject;

    private void OnEnable()
    {
        playerInteractor.OnFocus += InitializeFocusableObject;
    }

    private void InitializeFocusableObject(InteractableObject interactableObject)
    {
        Unsubscribe();

        _cameraFocusableObject = interactableObject.FocusableObject;
        
        _interactableObject = interactableObject;

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
        
        _interactableObject.UIEventsService.GameLoopScreen.rootVisualElement.style.display = DisplayStyle.Flex;
        _interactableObject.UIEventsService.InitializeGuide(null, null, null, null);
        // _interactableObject.UIEventsService.SetHint("<color=white>Tab</color> - справочные материалы");
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