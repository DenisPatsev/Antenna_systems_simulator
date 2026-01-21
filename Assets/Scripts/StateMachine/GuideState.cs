using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GuideState : IState
{
    public GuideState(GameBootstrapper gameBootstrapper, UIEventsService uiEventsService, StateMachine stateMachine)
    {
        _gameBootstrapper = gameBootstrapper;
        _uiEventsService = uiEventsService;
        _stateMachine = stateMachine;
    }

    private GameBootstrapper _gameBootstrapper;
    private Dictionary<AntennaData, GameObject> _antennasList = new Dictionary<AntennaData, GameObject>();
    private UIEventsService _uiEventsService;
    private StateMachine _stateMachine;
    private Camera _camera;
    private ObjectsRotator _rotator;
    private int _currentIndex;

    private VisualElement _applicationsScopeContainer;
    private VisualElement _root;
    private Button _leftSwitch;
    private Button _rightSwitch;
    private Button _exitButton;
    private Label _frequencyLabel;
    private Label _apertureLabel;
    private Label _wavelengthLabel;
    private Label _nameLabel;
    private Label _descriptionLabel;

    public void Enter()
    {
        _currentIndex = 0;
        _antennasList.Clear();
        
        CreateCamera();

        Subscribe();

        for (int i = 0; i < _gameBootstrapper.AntennaDatas.Length; i++)
        {
            GameObject antenna =
                GameFactory.Instantiate(_gameBootstrapper.AntennaDatas[i].antennaModel, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));

            _antennasList.Add(_gameBootstrapper.AntennaDatas[i], antenna);
            
            antenna.transform.SetParent(_rotator.transform);
            antenna.transform.localPosition = _gameBootstrapper.AntennaDatas[i].guideModePosition;
            antenna.transform.localEulerAngles = _gameBootstrapper.AntennaDatas[i].guideModeRotation;
        }
        
        ShowCurrentAntenna();
    }

    private void CreateCamera()
    {
        GameObject cameraObject = GameFactory.CreateObject(Constants.GuideCameraPath, Vector3.zero, Quaternion.identity);
        _camera = cameraObject.GetComponent<Camera>();
        _rotator = cameraObject.GetComponentInChildren<ObjectsRotator>();
    }

    public void Exit()
    {
        Unsubscribe();
    }

    private void OnLeftSwitchButtonClicked()
    {
        _currentIndex--;

        if (_currentIndex < 0)
            _currentIndex = _gameBootstrapper.AntennaDatas.Length - 1;
        
        ShowCurrentAntenna();
        CreateApplicationsScopeInfo();
    }

    private void OnRightSwitchButtonClicked()
    {
        _currentIndex++;
        
        if(_currentIndex >= _gameBootstrapper.AntennaDatas.Length)
            _currentIndex = 0;
        
        ShowCurrentAntenna();
        CreateApplicationsScopeInfo();
    }

    private void CreateApplicationsScopeInfo()
    {
        _applicationsScopeContainer.contentContainer.Clear();

        for (int i = 0; i < _gameBootstrapper.AntennaDatas[_currentIndex]._applicationsScope.Length; i++)
        {
            TemplateContainer temp = _uiEventsService.ApplicationScopeTemplate.Instantiate();
            Label applicationItem = temp.Q<Label>("Application");
            applicationItem.text = _gameBootstrapper.AntennaDatas[_currentIndex]._applicationsScope[i];
            
            _applicationsScopeContainer.Add(temp);
        }
    }

    private void UpdateInfo()
    {
        _nameLabel.text = _gameBootstrapper.AntennaDatas[_currentIndex].antennaName;
        _frequencyLabel.text = _gameBootstrapper.AntennaDatas[_currentIndex].frequencyGHz.ToString() + " GHz";
        _apertureLabel.text = _gameBootstrapper.AntennaDatas[_currentIndex].aperture.ToString() + " m";
        _wavelengthLabel.text = _gameBootstrapper.AntennaDatas[_currentIndex].wavelength.ToString() + " m";
        _descriptionLabel.text = _gameBootstrapper.AntennaDatas[_currentIndex].antennaDescription;
    }

    private void ShowCurrentAntenna()
    {
        HideAllAntennas();
        _rotator.Initialize(_antennasList[_gameBootstrapper.AntennaDatas[_currentIndex]]);

        _antennasList[_gameBootstrapper.AntennaDatas[_currentIndex]].SetActive(true);
        
        UpdateInfo();
    }

    private void OnExitButtonClicked()
    {
        _gameBootstrapper.SceneLoader.LoadScene("MenuScene", OnMainMenuSceneLoaded);
    }

    private void HideAllAntennas()
    {
        foreach (var antenna in _antennasList.Values)
        {
            antenna.SetActive(false);
        }
    }

    private void OnMainMenuSceneLoaded()
    {
        _stateMachine.Enter<MainMenuState>();
    }

    public void Subscribe()
    {
        _uiEventsService.MainMenuScreen.gameObject.SetActive(false);
        _uiEventsService.GuideScreen.gameObject.SetActive(true);
        
        _root = _uiEventsService.GuideScreen.rootVisualElement;
        _applicationsScopeContainer =
            _root.Q<VisualElement>("ApplicationsScopeContainer").Q<VisualElement>("ContentContainer");
        _frequencyLabel = _root.Q<Label>("Frequency");
        _apertureLabel = _root.Q<Label>("Aperture");
        _wavelengthLabel = _root.Q<Label>("Wavelength");
        _nameLabel = _root.Q<Label>("AntennaName");
        _descriptionLabel = _root.Q<Label>("Description");
        _leftSwitch = _root.Q<Button>("LeftSwitch");
        _rightSwitch = _root.Q<Button>("RightSwitch");
        _exitButton = _root.Q<Button>("ExitButton");
        
        _leftSwitch.clicked += OnLeftSwitchButtonClicked;
        _rightSwitch.clicked += OnRightSwitchButtonClicked;
        _exitButton.clicked += OnExitButtonClicked;

        CreateApplicationsScopeInfo();
    }

    public void Unsubscribe()
    {
        _leftSwitch.clicked -= OnLeftSwitchButtonClicked;
        _rightSwitch.clicked -= OnRightSwitchButtonClicked;
        _exitButton.clicked -= OnExitButtonClicked;
        
        _uiEventsService.GuideScreen.gameObject.SetActive(false);
    }
}