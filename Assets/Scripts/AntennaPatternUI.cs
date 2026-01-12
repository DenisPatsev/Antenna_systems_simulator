using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AntennaPatternUI : MonoBehaviour
{
    [Header("Data Source")]
    [Tooltip("Drag and drop your data file here")]
    public TextAsset dataFile;
    
    [Header("Settings")]
    public float scale = 5f;
    public float lineWidth = 0.02f;
    public Color patternColor = Color.red;
    public bool showReferenceCircles = true;
    public int numberOfCircles = 5;
    
    [Header("References")]
    public Material lineMaterial;
    
    private List<Vector2> points = new List<Vector2>();
    private LineRenderer lineRenderer;
    
    void Start()
    {
        if (dataFile != null)
        {
            ParseDataFromTextAsset();
            CreatePatternVisualization();
        }
        else
        {
            Debug.LogWarning("No data file assigned. Please assign a text file in the inspector.");
        }
    }
    
    void ParseDataFromTextAsset()
    {
        points.Clear();
        
        if (dataFile == null)
        {
            Debug.LogError("Data file is null!");
            return;
        }
        
        try
        {
            string[] lines = dataFile.text.Split('\n');
            
            foreach (string line in lines)
            {
                // Пропускаем пустые строки
                if (string.IsNullOrEmpty(line.Trim()))
                    continue;
                
                // Разделяем строку по пробелам или табам
                string[] parts = line.Split(new char[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                
                if (parts.Length >= 2)
                {
                    if (float.TryParse(parts[0], out float angle) && 
                        float.TryParse(parts[1], out float gain))
                    {
                        // Преобразуем полярные координаты в декартовы
                        float radians = angle * Mathf.Deg2Rad;
                        float radius = Mathf.Pow(10, gain / 20f) * scale; // Преобразуем dB в линейный масштаб
                        
                        Vector2 point = new Vector2(
                            Mathf.Cos(radians) * radius,
                            Mathf.Sin(radians) * radius
                        );
                        
                        points.Add(point);
                    }
                }
            }
            
            Debug.Log($"Successfully parsed {points.Count} data points from {dataFile.name}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error parsing data: {e.Message}");
        }
    }
    
    void CreatePatternVisualization()
    {
        if (points.Count == 0)
            return;
        
        // Создаем объект для визуализации
        GameObject patternObject = new GameObject("AntennaPattern");
        patternObject.transform.SetParent(this.transform);
        
        // Добавляем LineRenderer
        lineRenderer = patternObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = points.Count + 1; // +1 для замыкания контура
        lineRenderer.loop = true;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = patternColor;
        lineRenderer.endColor = patternColor;
        
        // Устанавливаем позиции точек
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, new Vector3(points[i].x, points[i].y, 0));
        }
        
        // Замыкаем контур
        lineRenderer.SetPosition(points.Count, new Vector3(points[0].x, points[0].y, 0));
        
        // Создаем опорные круги
        if (showReferenceCircles)
        {
            CreateReferenceCircles();
        }
        
        // Создаем оси координат
        CreateCoordinateAxes();
        
        // Добавляем информационный текст
        CreateInfoText();
    }
    
    void CreateReferenceCircles()
    {
        GameObject circlesParent = new GameObject("ReferenceCircles");
        circlesParent.transform.SetParent(this.transform);
        
        for (int i = 1; i <= numberOfCircles; i++)
        {
            float radius = (scale / numberOfCircles) * i;
            
            GameObject circleObject = new GameObject($"Circle_{i}");
            circleObject.transform.SetParent(circlesParent.transform);
            
            LineRenderer circleRenderer = circleObject.AddComponent<LineRenderer>();
            circleRenderer.loop = true;
            circleRenderer.startWidth = 0.01f;
            circleRenderer.endWidth = 0.01f;
            circleRenderer.material = lineMaterial;
            circleRenderer.startColor = Color.gray;
            circleRenderer.endColor = Color.gray;
            
            // Создаем круг из 36 сегментов
            int segments = 36;
            circleRenderer.positionCount = segments;
            
            for (int j = 0; j < segments; j++)
            {
                float angle = j * 360f / segments * Mathf.Deg2Rad;
                Vector3 point = new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0
                );
                circleRenderer.SetPosition(j, point);
            }
            
            // Добавляем текстовую метку
            CreateCircleLabel(radius, i);
        }
    }
    
    void CreateCircleLabel(float radius, int circleNumber)
    {
        GameObject labelObject = new GameObject($"Label_{circleNumber}");
        labelObject.transform.SetParent(this.transform);
        labelObject.transform.position = new Vector3(radius + 0.2f, 0, 0);
        
        TextMesh textMesh = labelObject.AddComponent<TextMesh>();
        textMesh.text = $"{circleNumber * 10} dB";
        textMesh.characterSize = 0.1f;
        textMesh.fontSize = 20;
        textMesh.color = Color.gray;
        textMesh.anchor = TextAnchor.MiddleLeft;
    }
    
    void CreateCoordinateAxes()
    {
        GameObject axesParent = new GameObject("CoordinateAxes");
        axesParent.transform.SetParent(this.transform);
        
        // Ось X
        CreateAxisLine("X_Axis", new Vector3(-scale * 1.2f, 0, 0), new Vector3(scale * 1.2f, 0, 0), Color.white);
        
        // Ось Y
        CreateAxisLine("Y_Axis", new Vector3(0, -scale * 1.2f, 0), new Vector3(0, scale * 1.2f, 0), Color.white);
        
        // Центральная точка
        GameObject centerPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        centerPoint.name = "Center";
        centerPoint.transform.SetParent(axesParent.transform);
        centerPoint.transform.localScale = Vector3.one * 0.05f;
        centerPoint.transform.position = Vector3.zero;
        
        Renderer renderer = centerPoint.GetComponent<Renderer>();
        renderer.material.color = Color.white;
    }
    
    void CreateAxisLine(string name, Vector3 start, Vector3 end, Color color)
    {
        GameObject lineObject = new GameObject(name);
        lineObject.transform.SetParent(this.transform);
        
        LineRenderer axisRenderer = lineObject.AddComponent<LineRenderer>();
        axisRenderer.positionCount = 2;
        axisRenderer.SetPosition(0, start);
        axisRenderer.SetPosition(1, end);
        axisRenderer.startWidth = 0.01f;
        axisRenderer.endWidth = 0.01f;
        axisRenderer.material = lineMaterial;
        axisRenderer.startColor = color;
        axisRenderer.endColor = color;
    }
    
    void CreateInfoText()
    {
        GameObject infoObject = new GameObject("PatternInfo");
        infoObject.transform.SetParent(this.transform);
        infoObject.transform.position = new Vector3(-scale * 1.1f, scale * 1.1f, 0);
        
        TextMesh textMesh = infoObject.AddComponent<TextMesh>();
        textMesh.text = $"File: {dataFile.name}\nPoints: {points.Count}";
        textMesh.characterSize = 0.05f;
        textMesh.fontSize = 20;
        textMesh.color = Color.yellow;
        textMesh.anchor = TextAnchor.UpperLeft;
    }
    
    // Метод для перезагрузки данных
    public void ReloadData()
    {
        // Удаляем старую визуализацию
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        if (dataFile != null)
        {
            ParseDataFromTextAsset();
            CreatePatternVisualization();
        }
    }
    
    // Метод для изменения масштаба
    public void SetScale(float newScale)
    {
        scale = newScale;
        ReloadData();
    }
    
    // Метод для смены файла данных
    public void SetDataFile(TextAsset newDataFile)
    {
        dataFile = newDataFile;
        ReloadData();
    }
    
    // Метод для проверки наличия данных
    public bool HasData()
    {
        return points.Count > 0;
    }
    
    // Метод для получения информации о данных
    public string GetDataInfo()
    {
        if (dataFile == null) return "No file assigned";
        return $"File: {dataFile.name}, Points: {points.Count}";
    }
}