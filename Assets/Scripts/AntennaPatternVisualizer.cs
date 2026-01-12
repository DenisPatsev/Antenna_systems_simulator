using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using UnityEngine;

public class AntennaPatternVisualizer : MonoBehaviour
{
    [Header("Data Source")]
    [Tooltip("Drag and drop your data file here")]
    public TextAsset dataFile;
    
    [Header("Settings")]
    public float scale = 2f;
    public float lineWidth = 0.02f;
    public Color patternColor = Color.red;
    
    [Header("Scale Settings")]
    public float referenceGain = 0f; // Усиление, которое будет на краю графика
    // public float dynamicScaling = true; // Автоматически подбирать масштаб
    
    [Header("References")]
    public Material lineMaterial;
    
    private List<Vector2> points = new List<Vector2>();
    private LineRenderer lineRenderer;
    private float dataMaxGain = 0f;
    
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
            dataMaxGain = float.MinValue;
            List<float> gains = new List<float>();
            List<float> angles = new List<float>();
            
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex].Trim();
                
                // Пропускаем пустые строки и комментарии
                if (string.IsNullOrEmpty(line) || line.StartsWith("#") || line.StartsWith("//"))
                    continue;
                
                // Разные варианты разделителей
                string[] parts = line.Split(new char[] { ' ', '\t', ',', ';' }, System.StringSplitOptions.RemoveEmptyEntries);
                
                if (parts.Length >= 2)
                {
                    // Пробуем разные форматы чисел
                    float angle = 0f;
                    float gain = 0f;
                    
                    if (TryParseFloat(parts[0], out angle) && TryParseFloat(parts[1], out gain))
                    {
                        angles.Add(angle);
                        gains.Add(gain);
                        
                        if (gain > dataMaxGain)
                            dataMaxGain = gain;
                    }
                }
            }
            
            // Автоматически настраиваем referenceGain если нужно
            // if (dynamicScaling)
            // {
            //     referenceGain = dataMaxGain - 20f; // Отображаем диапазон 20 dB от максимума
            // }
            
            // Создаем точки
            for (int i = 0; i < angles.Count; i++)
            {
                CreatePoint(angles[i], gains[i]);
            }
            
            Debug.Log($"Successfully parsed {points.Count} data points");
            Debug.Log($"Max gain: {dataMaxGain}, Reference gain: {referenceGain}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error parsing data: {e.Message}\n{e.StackTrace}");
        }
    }
    
    bool TryParseFloat(string input, out float result)
    {
        // Пробуем разные форматы и культуры
        if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            return true;
            
        // Пробуем заменить запятые на точки и наоборот
        string normalizedInput = input.Replace(',', '.');
        if (float.TryParse(normalizedInput, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            return true;
            
        normalizedInput = input.Replace('.', ',');
        if (float.TryParse(normalizedInput, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            return true;
            
        return false;
    }
    
    void CreatePoint(float angle, float gain)
    {
        // Преобразуем полярные координаты в декартовы
        float radians = angle * Mathf.Deg2Rad;
        
        // Вычисляем относительное усиление относительно referenceGain
        float relativeGain = gain - referenceGain;
        
        // Ограничиваем минимальное значение чтобы избежать отрицательных радиусов
        if (relativeGain < -50f) relativeGain = -50f;
        
        // Преобразуем dB в линейный масштаб
        // dB -> linear: magnitude = 10^(dB/20)
        float magnitude = Mathf.Pow(10f, relativeGain / 20f);
        
        // Применяем масштаб
        float radius = magnitude * scale;
        
        Vector2 point = new Vector2(
            Mathf.Cos(radians) * radius,
            Mathf.Sin(radians) * radius
        );
        
        points.Add(point);
    }
    
    void CreatePatternVisualization()
    {
        if (points.Count == 0)
        {
            Debug.LogError("No points to visualize!");
            return;
        }
        
        // Удаляем старую визуализацию
        ClearVisualization();
        
        // Создаем объект для визуализации
        GameObject patternObject = new GameObject("AntennaPattern");
        patternObject.transform.SetParent(this.transform);
        patternObject.transform.localPosition = Vector3.zero;
        
        // Добавляем LineRenderer
        lineRenderer = patternObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = points.Count;
        lineRenderer.loop = true;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = patternColor;
        lineRenderer.endColor = patternColor;
        lineRenderer.useWorldSpace = false;
        
        // Устанавливаем позиции точек
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, new Vector3(points[i].x, points[i].y, 0));
        }
        
        // Создаем информационный текст
        CreateInfoText();
    }
    
    void CreateInfoText()
    {
        GameObject infoObject = new GameObject("PatternInfo");
        infoObject.transform.SetParent(this.transform);
        infoObject.transform.localPosition = new Vector3(-scale * 0.8f, scale * 0.8f, 0);
        
        TextMesh textMesh = infoObject.AddComponent<TextMesh>();
        textMesh.text = $"Max: {dataMaxGain:F1} dB\nRef: {referenceGain:F1} dB";
        textMesh.characterSize = 0.04f;
        textMesh.fontSize = 16;
        textMesh.color = Color.yellow;
        textMesh.anchor = TextAnchor.UpperLeft;
    }
    
    void ClearVisualization()
    {
        // Удаляем старую визуализацию
        foreach (Transform child in transform)
        {
            if (child != this.transform)
                DestroyImmediate(child.gameObject);
        }
    }
    
    // Метод для перезагрузки данных
    [ContextMenu("Reload Data")]
    public void ReloadData()
    {
        ClearVisualization();
        
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
    
    // Метод для изменения reference gain
    public void SetReferenceGain(float newReferenceGain)
    {
        referenceGain = newReferenceGain;
        // dynamicScaling = false;
        ReloadData();
    }
    
    // Метод для смены файла данных
    public void SetDataFile(TextAsset newDataFile)
    {
        dataFile = newDataFile;
        ReloadData();
    }
}