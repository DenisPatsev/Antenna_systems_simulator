using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AntennaData", menuName = "Scriptable Objects/AntennaData")]
public class AntennaData : ScriptableObject
{
    public string antennaName;
    public string antennaDescription;
    public float fraungoferDistance;
    public float frequencyGHz;
    public float aperture;
    public float wavelength;
    public float diagram3DScale;
    public TextAsset diagram2DDatafile;
    public TextAsset diagram3DDatafile;
    public Texture2D diagram2DImage;
    public GameObject antennaModel;
    public Vector3 antennaModelPosition;
    public Vector3 antennaModelRotation;
    public Vector3 guideModePosition;
    public Vector3 guideModeRotation;
    public Vector3 diagram3DRotation;
    public Vector3 diagram3DPosition;
    public List<float> signalLevelValues = new List<float>();
    public string[] _applicationsScope;
}
