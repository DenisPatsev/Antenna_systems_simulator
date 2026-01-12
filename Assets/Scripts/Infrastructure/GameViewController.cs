using UnityEngine;

public class GameViewController  : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Camera _computerCamera;

    public void EnableComputerView()
    {
        _mainCamera.enabled = false;
        _computerCamera.enabled = true;
    }

    public void EnableLaboratoryView()
    {
        _mainCamera.enabled = true;
        _computerCamera.enabled = false;
    }
}