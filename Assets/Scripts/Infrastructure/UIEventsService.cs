using System;
using System.Collections;
using InteractableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIEventsService : MonoBehaviour
{
    [SerializeField] private UIDocument _diagramViewScreen;
    [SerializeField] private UIDocument _MainMenuScreen;
    [SerializeField] private UIDocument _gameLoopScreen;
    [SerializeField] private UIDocument _guideScreen;
    [SerializeField] private VisualTreeAsset _ApplicationScopeTemplate;
    [SerializeField] private SimpleWorldPointer _pointer;

    public UIDocument DiagramScreen => _diagramViewScreen;
    public UIDocument MainMenuScreen => _MainMenuScreen;
    public UIDocument GameLoopScreen => _gameLoopScreen;
    public UIDocument GuideScreen => _guideScreen;
    public VisualTreeAsset ApplicationScopeTemplate => _ApplicationScopeTemplate;
    public GameObject Blocker => _blocker;
    public Canvas FraungoferDistanceCanvas => _fraungoferDistanceCanvas;
    public PlayerController PlayerController => _playerController;
    public SimpleWorldPointer Pointer => _pointer;

    public UnityAction OnGameExit;


    private GameObject _blocker;
    private PlayerInteractor _playerInteractor;
    private InfoCanvas _infoCanvas;
    private Canvas _fraungoferDistanceCanvas;
    private PlayerController _playerController;
    private Door _door;
    private Label _keyHint;
    private Label _antennaNameLabel;
    private Label _frequency;
    private Label _apertureLabel;
    private Label _wavelengthLabel;
    private VisualElement _root;
    private VisualElement _guide;
    private string _antennaName = null;
    private string _antennaFrequency = null;
    private string _aperture = null;
    private string _wavelength = null;

    private Computer _computer;

    public Computer Computer => _computer;

    private void OnDisable()
    {
        if (_playerInteractor == null && _infoCanvas == null)
            return;

        _playerInteractor.OnInteract -= _infoCanvas.DisplayInfo;
        _playerInteractor.OnInteractEnd -= _infoCanvas.HideInfo;

        if (_door != null)
            _door.OnGameExit -= OnExitGameStart;
    }

    private void Update()
    {
        if (_guide == null)
            return;

        if (Input.GetKeyDown(Constants.GuideKeyCode))
            _guide.style.display = DisplayStyle.Flex;
        else if (Input.GetKeyUp(Constants.GuideKeyCode))
            _guide.style.display = DisplayStyle.None;
    }

    private void OnExitGameStart()
    {
        OnGameExit?.Invoke();
    }

    private IEnumerator GuideInitializer(string antennaName, string frequency, string aperture, string wavelength)
    {
        while (_root == null || _gameLoopScreen.gameObject.activeSelf == false)
            yield return null;

        _guide = _gameLoopScreen.rootVisualElement.Q<VisualElement>("GuideContainer");
        _antennaNameLabel = _guide.Q<Label>("AntennaName");
        _frequency = _guide.Q<Label>("Frequency");
        _apertureLabel = _guide.Q<Label>("Aperture");
        _wavelengthLabel = _guide.Q<Label>("Wavelength");
        
        if(antennaName != null)
            _antennaName = antennaName;
        
        if(frequency != null)
            _antennaFrequency = frequency;
        
        if(aperture != null)
            _aperture = aperture;
        
        if(wavelength != null)
            _wavelength = wavelength;

        if (_antennaName == null)
            _antennaNameLabel.text = antennaName;
        else
            _antennaNameLabel.text = _antennaName;

        if (_antennaFrequency == null)
            _frequency.text = frequency + " ГГц";
        else
            _frequency.text = _antennaFrequency + " ГГц";
        
        if(_aperture != null)
            _apertureLabel.text = _aperture + " m";
        else
            _apertureLabel.text = aperture + " m";
        
        if(_wavelength != null)
            _wavelengthLabel.text = _wavelength + " m";
        else
            _wavelengthLabel.text = wavelength + " m";
    }

    public void ResetUIDocumentsState()
    {
        _gameLoopScreen.gameObject.SetActive(false);
        _MainMenuScreen.gameObject.SetActive(true);
        _pointer.enabled = false;
    }

    public void InitializeComputer(Computer computer)
    {
        _computer = computer;
    }

    public void InitializeGuide(string antennaName, string frequency, string aperture, string wavelength)
    {
        StartCoroutine(GuideInitializer(antennaName, frequency, aperture, wavelength));
    }

    public void InitializeBlocker(GameObject blocker)
    {
        _blocker = blocker;
    }

    public void InitializeHintLabel()
    {
        _root = _gameLoopScreen.rootVisualElement;
        _keyHint = _root.Q<Label>("ActionHint");
    }

    public void SetHint(string hintText)
    {
        _keyHint.text = hintText;
    }

    public void InitializePlayerController(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public void InitializeFraungoferDistanceCanvas(Canvas canvas)
    {
        _fraungoferDistanceCanvas = canvas;
    }

    public void InitializeMainSceneObjects(PlayerInteractor interactor, InfoCanvas infoCanvas)
    {
        _playerInteractor = interactor;
        _infoCanvas = infoCanvas;

        _playerInteractor.OnInteract += _infoCanvas.DisplayInfo;
        _playerInteractor.OnInteractEnd += _infoCanvas.HideInfo;
    }

    public void InitializeDoor()
    {
        _door = FindAnyObjectByType(typeof(Door)) as Door;

        if (_door != null)
            _door.OnGameExit += OnExitGameStart;
    }
}