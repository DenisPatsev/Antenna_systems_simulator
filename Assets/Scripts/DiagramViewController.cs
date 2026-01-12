using UnityEngine;

public class DiagramViewController : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _maxZoom;
    [SerializeField] private float _minZoom;
    [SerializeField] private Camera _camera;
    [SerializeField] private DiagramBuilder _diagramBuilder;

    private float _zOffset;

    private void OnEnable()
    {
        if(_camera == null)
            Instantiate(new Camera(), transform);
        
        _zOffset = _camera.transform.localPosition.z;
        CalculateCameraTransform();
    }

    private void Update()
    {
        if(_camera == null)
            return;
        
        if (Input.GetKey(KeyCode.Mouse0))
        {
            _zOffset += _zoomSpeed * -Input.GetAxis("Mouse Y") * Time.deltaTime;

            CalculateCameraTransform();

            transform.rotation *= Quaternion.Euler(Vector3.up * _rotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime);
        }

        if (_diagramBuilder.Labels.Count == 0)
            return;

        foreach (var label in _diagramBuilder.Labels)
        {
            label.transform.LookAt(_camera.transform);
        }
    }

    private void CalculateCameraTransform()
    {
        _zOffset = Mathf.Clamp(_zOffset, _minZoom, _maxZoom);

        _camera.transform.localPosition = new Vector3(_camera.transform.localPosition.x,
            _camera.transform.localPosition.y, _zOffset);

        _camera.transform.LookAt(transform.position);
    }
}