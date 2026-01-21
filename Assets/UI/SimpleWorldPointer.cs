using UnityEngine;
using UnityEngine.UIElements;

public class SimpleWorldPointer : MonoBehaviour
{
    [SerializeField] private Transform targetObject;
    [SerializeField] private string pointerName = "Pointer";
    [SerializeField] private float edgeOffset = 50f; // Отступ от краев экрана
    [SerializeField] private float _scalingSpeed;
    [SerializeField] private float _minScale;
    [SerializeField] private float _maxScale;
    
    private VisualElement pointer;
    private Camera cam;
    private UIDocument uiDocument;
    private float _currentScale;
    private float _targetScale;

    private void OnEnable()
    {
        cam = Camera.main;
        uiDocument = GetComponent<UIDocument>();
        pointer = uiDocument.rootVisualElement.Q<VisualElement>(pointerName);
        
        if (pointer != null)
        {
            pointer.style.position = Position.Absolute;
            pointer.style.display = DisplayStyle.None;
        }
        
        _targetScale = _minScale;
    }

    private void OnDisable()
    {
        if (pointer != null)
            pointer.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        if (pointer == null || targetObject == null || uiDocument == null)
            return;

        Vector3 screenPos = cam.WorldToScreenPoint(targetObject.position);
        
        _currentScale = Mathf.MoveTowards(_currentScale, _targetScale, Time.deltaTime * _scalingSpeed);
        pointer.style.scale = new Vector2(_currentScale, _currentScale);
        
        if(_currentScale < _minScale + 0.01f)
            _targetScale = _maxScale;
        else if (_currentScale > _maxScale - 0.01f) 
            _targetScale = _minScale;
        
        if (screenPos.z <= 0)
        {
            pointer.style.display = DisplayStyle.None;
            return;
        }

        var root = uiDocument.rootVisualElement;
        
        Vector2 normalizedPos = new Vector2(
            screenPos.x / Screen.width,
            (Screen.height - screenPos.y) / Screen.height
        );
        
        float uiX = normalizedPos.x * root.resolvedStyle.width;
        float uiY = normalizedPos.y * root.resolvedStyle.height;
        
        pointer.style.left = uiX - pointer.resolvedStyle.width * 0.5f;
        pointer.style.top = uiY - pointer.resolvedStyle.height * 0.5f;
        pointer.style.display = DisplayStyle.Flex;
    }

    private void OnRectTransformDimensionsChange()
    {
        if (pointer != null && pointer.style.display == DisplayStyle.Flex)
        {
            Update();
        }
    }

    public void SetTargetObject(Transform target)
    {
        targetObject = target;
    }
}