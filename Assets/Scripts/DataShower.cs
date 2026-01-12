using System;
using System.Collections.Generic;
using UnityEngine;

public class DataShower : MonoBehaviour
{
    [SerializeField] private TextAsset textAsset;

    private List<float> values = new List<float>();

    private void Start()
    {
        values = DataParserStatic.GetDataFromFile(textAsset);

        foreach (float value in values)
        {
            Debug.Log(value);
        }
    }
}