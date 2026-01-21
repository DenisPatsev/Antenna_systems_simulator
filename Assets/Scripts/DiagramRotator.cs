using UnityEngine;

public class DiagramRotator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;

    private float _currentRotationY;
    
    private void Update()
    {
        _currentRotationY += Time.deltaTime * _rotationSpeed;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, _currentRotationY, transform.eulerAngles.z);
    }
}
