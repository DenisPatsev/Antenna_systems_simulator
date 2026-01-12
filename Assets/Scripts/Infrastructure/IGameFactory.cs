using UnityEngine;

public interface IGameFactory
{
    GameObject CreatePlayer(string path, Vector3 position);
    GameObject CreateInfoCanvas(string path, Vector3 position);
}
