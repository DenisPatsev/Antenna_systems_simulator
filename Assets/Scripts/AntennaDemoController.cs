using System;
using UnityEngine;

public class AntennaDemoController : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _cameraMoveSpeed;
    [SerializeField] private GameObject _antenna;
    [SerializeField] private Camera _camera;

    private float _rotX;
    private float _rotY;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            _rotX += -Input.GetAxis("Mouse Y") * _rotationSpeed;
            _rotY += Input.GetAxis("Mouse X") * _rotationSpeed;

            _antenna.transform.localRotation = Quaternion.Euler(_rotX, _rotY, 0);
        }

        _camera.transform.position += new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") * _cameraMoveSpeed);
    }
}