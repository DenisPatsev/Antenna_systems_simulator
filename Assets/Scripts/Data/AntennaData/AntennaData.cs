using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AntennaData", menuName = "Scriptable Objects/AntennaData")]
public class AntennaData : ScriptableObject
{
    public string antennaName;
    public float fraungoferDistance;
    public float frequencyGHz;
    public float maxdB;
    public float mindB;
    public float maxMHz;
    public float minMHz;
    public TextAsset diagram2DDatafile;
    public Texture2D diagram2DImage;
    public GameObject antennaModel;
    public List<float> signalLevelValues = new List<float>();
}
