using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public static class DataParserStatic
{
    public static List<float> GetDataFromFile(TextAsset textAsset)
    {
        List<float> values = new List<float>();
        
        if (textAsset == null)
        {
            Debug.LogWarning("TextAsset is null!");
            return values;
        }

        string[] lines = textAsset.text.Split('\n');
        
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string trimmedLine = line.Trim();
            
            string[] parts = trimmedLine.Split(new char[] { '\t', ' ' }, 
                System.StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length >= 2)
            {
                string valueStr = parts[1];
                
                valueStr = valueStr.Replace(',', '.');
                
                if (float.TryParse(valueStr, NumberStyles.Float, 
                        CultureInfo.InvariantCulture, out float value))
                {
                    values.Add(value);
                }
                else
                {
                    Debug.LogError($"Failed to parse value: {parts[1]} (converted to: {valueStr})");
                }
            }
        }
        
        Debug.Log($"Parsed {values.Count} values from {textAsset.name}");
        return values;
    }
}