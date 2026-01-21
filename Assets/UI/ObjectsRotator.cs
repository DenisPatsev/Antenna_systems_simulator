using UnityEngine;

public class ObjectsRotator : MonoBehaviour
{
    [SerializeField] private GameObject _rotatableObject;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private Camera _camera;

    private float _rotX;

    private void Update()
    {
        _rotX += _rotationSpeed * Time.deltaTime;
        _rotatableObject.transform.localEulerAngles = new Vector3(_rotatableObject.transform.localEulerAngles.x, _rotX, _rotatableObject.transform.localEulerAngles.z);
    }

    public void Initialize(GameObject rotatableObject)
    {
        _rotatableObject = rotatableObject;
    }
}