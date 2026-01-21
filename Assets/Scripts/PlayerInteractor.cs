using UnityEngine;
using UnityEngine.Events;

public class PlayerInteractor : MonoBehaviour
{
    public event UnityAction<string, Vector3> OnInteract;
    public event UnityAction<InteractableObject> OnFocus;
    public event UnityAction OnInteractEnd;

    private InteractableObject _interactableObject;
    private CameraFocusableObject _cameraFocusableObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out InteractableObject interactable))
        {
            interactable.SetOutline();
            Vector3 position = interactable.transform.position + interactable.InfoInterfacePosition;
            OnInteract?.Invoke(interactable.Name, position);
            _interactableObject = interactable;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(Constants.InteractionKeyCode))
        {
            if (_interactableObject != null)
            {
                _interactableObject.Interact();

                if (_interactableObject.FocusableObject != null && _interactableObject.FocusableObject.enabled)
                {
                    _interactableObject.FocusableObject.InitializeInteractor(this);
                    OnFocus?.Invoke(_interactableObject);
                    Debug.Log("Focused");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out InteractableObject interactable))
        {
            interactable.SetDefaultMaterial();
            OnInteractEnd?.Invoke();
            _interactableObject = null;
        }
    }
}