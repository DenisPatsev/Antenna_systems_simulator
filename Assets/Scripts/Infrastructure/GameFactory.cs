using UnityEngine;

public class GameFactory : MonoBehaviour
{
    private static GameObject _playerPrefab;
    private static GameObject _infoCanvasPrefab;

    public static GameObject CreatePlayer(string path, Vector3 position)
    {
        _playerPrefab = Resources.Load<GameObject>(path);
        GameObject player = Instantiate(_playerPrefab, position, Quaternion.identity);
        
        return player;
    }

    public static GameObject CreateInfoCanvas(string path, Vector3 position)
    {
        _infoCanvasPrefab = Resources.Load<GameObject>(path);
        GameObject canvas = Instantiate(_infoCanvasPrefab, position, Quaternion.identity);
        
        return canvas;
    }

    public static GameObject CreateObject(string path, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        GameObject obj = Instantiate(prefab, position, rotation);
        return obj;
    }
}