using Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AntennaRotator : MonoBehaviour, IStageObject
{
    [SerializeField] private InteractableUIButton _rightTurnButton;
    [SerializeField] private InteractableUIButton _leftTurnButton; 
    [SerializeField] private TMP_Text _degreesText;
    [SerializeField] private TMP_Text _dBLevelText;
    [SerializeField] private float _rotationstep;

    private float _currentRotationZ;
    private GameObject _trenogaWheel;
    
    public float RotationZ => _currentRotationZ;

    public event UnityAction OnInfoUpdated;
    public event UnityAction OnAllAnglesMeasured;

    private void OnEnable()
    {
        _rightTurnButton.OnInteract += OnRightButtonClick;
        _leftTurnButton.OnInteract += OnLeftButtonClick;
        
        LoadRotation();
    }

    private void OnDisable()
    {
        _rightTurnButton.OnInteract -= OnRightButtonClick;
        _leftTurnButton.OnInteract -= OnLeftButtonClick;
    }

    private void OnRightButtonClick()
    {
        RotateAntenna(1);
    }

    private void OnLeftButtonClick()
    {
        RotateAntenna(-1);
    }

    private void RotateAntenna(int direction)
    {
        _currentRotationZ += _rotationstep * direction;
        
        if(_currentRotationZ < 0)
            _currentRotationZ = 360 - _currentRotationZ;
        
        if(_currentRotationZ == 360)
            OnAllAnglesMeasured?.Invoke();

        _trenogaWheel.transform.localRotation = Quaternion.Euler(0, 0, _currentRotationZ);
        
        SaveRotation();
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        _degreesText.text = _currentRotationZ.ToString() + " deg.";
        
       OnInfoUpdated?.Invoke();
    }

    private void LoadRotation()
    {
        if (PlayerSessionData.CurrentAntennaRotationZ == 0)
            _currentRotationZ = 0;
        else
            _currentRotationZ = PlayerSessionData.CurrentAntennaRotationZ;
        
        UpdateInfo();
    }

    private void SaveRotation()
    {
        PlayerSessionData.CurrentAntennaRotationZ = _currentRotationZ;
    }

    public void InitializeWheel(GameObject wheel)
    {
        _trenogaWheel = wheel;
    }

    public void ShowSignalLevel(float level)
    {
        _dBLevelText.text = level.ToString();
    }
}