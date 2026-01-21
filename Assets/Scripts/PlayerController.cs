using Infrastructure.Data;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotaionSpeed;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private GameObject _player;
    [SerializeField] private Camera _camera;

    private Rigidbody _rigidbody;
    private float _yRotation;
    private float _xRotation;
    private float _horizontalAxisValue;
    private float _verticalAxisValue;
    private Vector3 _movement;

    private void OnEnable()
    {
        LoadTransform();
        // Debug.Log("PlayerData load! loaded data: position - " + PlayerData.PlayerPosition + ", rotation - " + PlayerData.PlayerRotation); 
    }

    private void OnDisable()
    {
        SaveTransform();
        // Debug.Log("PlayerData save! Saved data: position - " + PlayerData.PlayerPosition + ", rotation - " + PlayerData.PlayerRotation);
    }

    private void Update()
    {
        _horizontalAxisValue = Input.GetAxis("Horizontal");
        _verticalAxisValue = Input.GetAxis("Vertical");
        
        _yRotation += -Input.GetAxis("Mouse Y") * _rotaionSpeed * Time.deltaTime;
        _xRotation += Input.GetAxis("Mouse X") * _rotaionSpeed * Time.deltaTime;
        _yRotation = Mathf.Clamp(_yRotation, -90f, 90f);
        _movement = (_player.transform.forward * _verticalAxisValue + _player.transform.right * _horizontalAxisValue + _player.transform.up * -9.81f); 
        Vector3.Normalize(_movement);
        _characterController.Move(_movement * Time.deltaTime * _moveSpeed);
        
        _player.transform.rotation = Quaternion.Euler(0, _xRotation, 0);
        _camera.transform.localRotation = Quaternion.Euler(_yRotation, 0, 0);
    }

    public void SetRotationSpeedMultiplier(float speedMultiplier)
    {
        _rotaionSpeed *= speedMultiplier;
    }
    
    private void LoadTransform()
    {
        if (PlayerData.PlayerPosition != Vector3.zero)
            _player.transform.localPosition = PlayerData.PlayerPosition;
        else
        {
            _player.transform.localRotation = Quaternion.Euler(0, 0, 0);
            _camera.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        if (PlayerData.PlayerRotation.eulerAngles != Vector3.zero)
            _player.transform.localRotation = PlayerData.PlayerRotation;
    }

    private void SaveTransform()
    {
        PlayerData.PlayerPosition = _player.transform.localPosition;
        PlayerData.PlayerRotation = _player.transform.localRotation;
    }
}