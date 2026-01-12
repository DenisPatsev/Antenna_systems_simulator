using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

[CreateAssetMenu(fileName = "StageObjectsData", menuName = "Scriptable Objects/StageObjectsData")]
public class StageObjectsData : ScriptableObject
{
    public string StageName;
    public string Hint;
    public List<IStageObject> stageObjects = new List<IStageObject>();

    public IStageObject GetStageObjectByType(Type type)
    {
        foreach (var obj in stageObjects)
        {
            if (obj.GetType() == type)
            {
                return obj;
            }
        }
        
        return null;
    }
}
