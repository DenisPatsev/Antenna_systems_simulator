using UnityEngine;

public interface IObjectCreator
{
    GameObject CreateObject(string path, Vector3 position);
}