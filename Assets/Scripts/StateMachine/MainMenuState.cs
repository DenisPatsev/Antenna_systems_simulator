using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuState : IState
{
    private UIEventsService _uiEventsService;
    private GameBootstrapper _gameBootstrapper;
    private StateMachine _stateMachine;
    private VisualElement _root;
    private VisualElement _choiceScreen;
    private VisualElement _mainMenuScreen;
    private VisualElement _settingsScreen;
    private Button _measurementModeButton;
    private Button _settingsButton;
    private Button _dictionaryButton;
    private Button _exitButton;
    private Button _leftSwitchButton;
    private Button _rightSwitchButton;
    private Button _toMeasurementButton;
    private Button _settingsExitButton;
    private Toggle _bloomButton;
    private Toggle _aoButton;
    private Toggle _depthButton;
    private Label _antennaNameLabel;

    private int _currentAntennaIndex;

    public MainMenuState(GameBootstrapper gameBootstrapper, StateMachine stateMachine)
    {
        _uiEventsService = gameBootstrapper.UIEventsService;
        _gameBootstrapper = gameBootstrapper;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _uiEventsService.MainMenuScreen.gameObject.SetActive(true);
        _currentAntennaIndex = 0;

        Subscribe();
        UpdateAntennaInfo();
    }

    public void Exit()
    {
        _uiEventsService.MainMenuScreen.gameObject.SetActive(false);
        Unsubscribe();
    }

    private void OnMeasurementModeButtonClicked()
    {
        _mainMenuScreen.style.display = DisplayStyle.None;
        _settingsScreen.style.display = DisplayStyle.None;
        _choiceScreen.style.display = DisplayStyle.Flex;
    }

    private void OnSettingsButtonClicked()
    {
        _mainMenuScreen.style.display = DisplayStyle.None;
        _settingsScreen.style.display = DisplayStyle.Flex;
    }

    private void OnSettingsExitButtonClicked()
    {
        _settingsScreen.style.display = DisplayStyle.None;
        _mainMenuScreen.style.display = DisplayStyle.Flex;
    }

    private void OnDictionaryButtonClicked()
    {
    }

    private void OnBloomButtonClicked(ChangeEvent<bool> evt)
    {
        Constants.IsBloomOn = _bloomButton.value;
        Debug.Log("ValueChanged");
    }

    private void OnAOButtonClicked(ChangeEvent<bool> evt)
    {
        Constants.IsAOOn = _aoButton.value;
    }

    private void OnDepthButtonClicked(ChangeEvent<bool> evt)
    {
        Constants.IsDepthOfFieldOn = _depthButton.value;
    }

    private void OnExitButtonClicked()
    {
    }

    private void OnLeftSwitchButtonClicked()
    {
        _currentAntennaIndex--;

        if (_currentAntennaIndex < 0)
        {
            _currentAntennaIndex = _gameBootstrapper.AntennaDatas.Length - 1;
        }

        UpdateAntennaInfo();
    }

    private void OnRightSwitchButtonClicked()
    {
        _currentAntennaIndex++;

        if (_currentAntennaIndex >= _gameBootstrapper.AntennaDatas.Length)
        {
            _currentAntennaIndex = 0;
        }

        UpdateAntennaInfo();
    }

    private void OnApplyButtonClicked()
    {
        _gameBootstrapper.SetCurrentAntennaData(_gameBootstrapper.AntennaDatas[_currentAntennaIndex].antennaName);
        _gameBootstrapper.SceneLoader.LoadScene(Constants.MainSceneName, OnLoaded);
        Debug.Log(_gameBootstrapper.SelectedAntenna);
    }

    private void UpdateAntennaInfo()
    {
        _antennaNameLabel.text = _gameBootstrapper.AntennaDatas[_currentAntennaIndex].antennaName;
    }

    private void OnLoaded()
    {
        _stateMachine.Enter<GameLoopState>();
    }

    public void Subscribe()
    {
        _root = _uiEventsService.MainMenuScreen.rootVisualElement;

        _measurementModeButton = _root.Q<Button>("AntennaMeasurementButton");
        _settingsButton = _root.Q<Button>("SettingsButton");
        _dictionaryButton = _root.Q<Button>("LibraryButton");
        _exitButton = _root.Q<Button>("ExitButton");
        _leftSwitchButton = _root.Q<Button>("SwitchLeftButton");
        _rightSwitchButton = _root.Q<Button>("SwitchRightButton");
        _toMeasurementButton = _root.Q<Button>("ApplyButton");
        _choiceScreen = _root.Q<VisualElement>("AntennasChoice");
        _mainMenuScreen = _root.Q<VisualElement>("MainMenuContainer");
        _antennaNameLabel = _root.Q<Label>("AntennaNameLabel");
        _settingsScreen = _root.Q<VisualElement>("SettingsPanel");
        _bloomButton = _root.Q<VisualElement>("BloomEffectContainer").Q<Toggle>("EnableButton");
        _aoButton = _root.Q<VisualElement>("AOEffectContainer").Q<Toggle>("EnableButton");
        _depthButton = _root.Q<VisualElement>("DepthEffectContainer").Q<Toggle>("EnableButton");
        _settingsExitButton = _settingsScreen.Q<Button>("ExitButton");

        _measurementModeButton.clicked += OnMeasurementModeButtonClicked;
        _settingsButton.clicked += OnSettingsButtonClicked;
        _dictionaryButton.clicked += OnDictionaryButtonClicked;
        _exitButton.clicked += OnExitButtonClicked;
        _leftSwitchButton.clicked += OnLeftSwitchButtonClicked;
        _rightSwitchButton.clicked += OnRightSwitchButtonClicked;
        _toMeasurementButton.clicked += OnApplyButtonClicked;
        _settingsExitButton.clicked += OnSettingsExitButtonClicked;
        _bloomButton.RegisterValueChangedCallback(OnBloomButtonClicked);
        _aoButton.RegisterValueChangedCallback(OnAOButtonClicked);
        _depthButton.RegisterValueChangedCallback(OnDepthButtonClicked);
    }

    public void Unsubscribe()
    {
        _measurementModeButton.clicked -= OnMeasurementModeButtonClicked;
        _settingsButton.clicked -= OnSettingsButtonClicked;
        _dictionaryButton.clicked -= OnDictionaryButtonClicked;
        _exitButton.clicked -= OnExitButtonClicked;
        _leftSwitchButton.clicked -= OnLeftSwitchButtonClicked;
        _rightSwitchButton.clicked -= OnRightSwitchButtonClicked;
        _toMeasurementButton.clicked -= OnApplyButtonClicked;
        _settingsExitButton.clicked -= OnSettingsExitButtonClicked;
        _bloomButton.UnregisterValueChangedCallback(OnBloomButtonClicked);
        _aoButton.UnregisterValueChangedCallback(OnAOButtonClicked);
        _depthButton.UnregisterValueChangedCallback(OnDepthButtonClicked);
    }
}