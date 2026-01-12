using UnityEngine;

public class DiagramRotator : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationSpeed;

    private void Update()
    {
        transform.localRotation *= Quaternion.Euler(_rotationSpeed * Time.deltaTime);
    }
}
