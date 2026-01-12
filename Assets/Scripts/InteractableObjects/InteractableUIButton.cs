using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractableUIButton : MonoBehaviour
{
    public Color focusedColor;
    public Color normalColor;
    public Button button; 
    public event UnityAction OnInteract;
    
    private PlayerInteractor _playerInteractor;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerInteractor interactor))
        {
           _playerInteractor = interactor;
           gameObject.transform.localScale *= 1.2f;
           button.image.color = focusedColor;
        }
    }

    private void Update()
    {
        if(_playerInteractor == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            OnInteract?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerInteractor interactor))
        {
            _playerInteractor = null;
           gameObject.transform.localScale /= 1.2f;
           button.image.color = normalColor;
        }
    }
}