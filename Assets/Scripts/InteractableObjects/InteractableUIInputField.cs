using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractableUIInputField : MonoBehaviour
{
    public TMP_InputField inputField;

    private PlayerInteractor _playerInteractor;
    public event UnityAction OnInteract;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerInteractor interactor))
        {
            _playerInteractor = interactor;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Debug.Log(inputField.text);
        }

        if (_playerInteractor == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            inputField.Select();
            OnInteract?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerInteractor interactor))
        {
            _playerInteractor = null;
        }
    }
}