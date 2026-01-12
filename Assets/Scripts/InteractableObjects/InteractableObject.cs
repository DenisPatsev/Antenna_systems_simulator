using System;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class InteractableObject : MonoBehaviour, IInteractableObject
{
    [SerializeField] protected string _name;
    [SerializeField] protected MeshRenderer[] _meshRenderers;
    [SerializeField] protected Material _outlineMaterial;
    [SerializeField] protected Vector3 _infoInterfacePosition;
    [SerializeField] protected CameraFocusableObject _focusableObject;

    protected UIEventsService _uiEventsService;
    private Material[] _defaultMaterials;
    public Vector3 InfoInterfacePosition => _infoInterfacePosition;
    public string Name => _name;
    public CameraFocusableObject FocusableObject => _focusableObject;

    protected void OnEnable()
    {
        _defaultMaterials = new Material[_meshRenderers.Length];

        for (int i = 0; i < _defaultMaterials.Length; i++)
        {
            _defaultMaterials[i] = _meshRenderers[i].sharedMaterial;
        }

        if (_focusableObject != null)
        {
            _focusableObject.OnFocusStart += SetDefaultMaterial;
        }
    }

    protected void OnDisable()
    {
        if (_focusableObject != null)
        {
            _focusableObject.OnFocusStart -= SetDefaultMaterial;
        }
    }

    public void Initialize(UIEventsService uiEventsService)
    {
        _uiEventsService = uiEventsService;
    }

    public void SetOutline()
    {
        foreach (var meshRenderer in _meshRenderers)
            meshRenderer.sharedMaterial = _outlineMaterial;
    }

    public void SetDefaultMaterial()
    {
        for (int i = 0; i < _meshRenderers.Length; i++)
        {
            if (_defaultMaterials != null)
                _meshRenderers[i].sharedMaterial = _defaultMaterials[i];
        }
    }

    public virtual void Interact()
    {
        SetDefaultMaterial();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + _infoInterfacePosition, 0.2f);
    }
}