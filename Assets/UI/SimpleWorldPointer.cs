using UnityEngine;
using UnityEngine.UIElements;

public class SimpleWorldPointer : MonoBehaviour
{
    [SerializeField] private Transform targetObject;
    [SerializeField] private string pointerName = "Pointer";
    [SerializeField] private float edgeOffset = 50f; // Отступ от краев экрана
    
    private VisualElement pointer;
    private Camera cam;
    private UIDocument uiDocument;

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
        
        // Если объект позади камеры, скрываем указатель
        if (screenPos.z <= 0)
        {
            pointer.style.display = DisplayStyle.None;
            return;
        }

        // Получаем корневой элемент UI
        var root = uiDocument.rootVisualElement;
        
        // Конвертируем координаты в относительные (0-1 диапазон)
        Vector2 normalizedPos = new Vector2(
            screenPos.x / Screen.width,
            (Screen.height - screenPos.y) / Screen.height
        );
        
        // Конвертируем в координаты UI Toolkit
        float uiX = normalizedPos.x * root.resolvedStyle.width;
        float uiY = normalizedPos.y * root.resolvedStyle.height;
        
        // Устанавливаем позицию с центрированием указателя
        pointer.style.left = uiX - pointer.resolvedStyle.width * 0.5f;
        pointer.style.top = uiY - pointer.resolvedStyle.height * 0.5f;
        pointer.style.display = DisplayStyle.Flex;
    }

    // Альтернативный метод с ограничением указателя в пределах экрана
    private void UpdatePointerPositionWithBounds()
    {
        if (pointer == null || targetObject == null || uiDocument == null)
            return;

        Vector3 screenPos = cam.WorldToScreenPoint(targetObject.position);
        
        if (screenPos.z <= 0)
        {
            pointer.style.display = DisplayStyle.None;
            return;
        }

        var root = uiDocument.rootVisualElement;
        
        // Нормализованные координаты (0-1)
        Vector2 normalizedPos = new Vector2(
            Mathf.Clamp01(screenPos.x / Screen.width),
            Mathf.Clamp01((Screen.height - screenPos.y) / Screen.height)
        );
        
        // Конвертируем в пиксели UI
        float uiX = normalizedPos.x * root.resolvedStyle.width;
        float uiY = normalizedPos.y * root.resolvedStyle.height;
        
        // Получаем размеры указателя
        float pointerWidth = pointer.resolvedStyle.width;
        float pointerHeight = pointer.resolvedStyle.height;
        
        // Ограничиваем позицию, чтобы указатель не выходил за границы
        uiX = Mathf.Clamp(
            uiX - pointerWidth * 0.5f,
            edgeOffset,
            root.resolvedStyle.width - pointerWidth - edgeOffset
        );
        
        uiY = Mathf.Clamp(
            uiY - pointerHeight * 0.5f,
            edgeOffset,
            root.resolvedStyle.height - pointerHeight - edgeOffset
        );
        
        pointer.style.left = uiX;
        pointer.style.top = uiY;
        pointer.style.display = DisplayStyle.Flex;
    }

    // Метод для обновления при изменении разрешения
    private void OnRectTransformDimensionsChange()
    {
        if (pointer != null && pointer.style.display == DisplayStyle.Flex)
        {
            // Пересчитываем позицию при изменении размеров
            Update();
        }
    }

    public void SetTargetObject(Transform target)
    {
        targetObject = target;
        
        // Принудительное обновление позиции при смене цели
        if (pointer != null && target != null)
        {
            Update();
        }
    }
}