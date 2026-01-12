using InteractableObjects;
using UnityEngine;
using UnityEngine.UIElements;

public class UIEventsService : MonoBehaviour
{
    [SerializeField] private UIDocument _diagramViewScreen;
    [SerializeField] private UIDocument _MainMenuScreen;
    [SerializeField] private UIDocument _gameLoopScreen;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private SimpleWorldPointer _pointer;
    
    public UIDocument DiagramScreen => _diagramViewScreen;
    public UIDocument MainMenuScreen => _MainMenuScreen;
    public UIDocument GameLoopScreen => _gameLoopScreen;
    public GameObject Blocker  => _blocker;
    public Canvas FraungoferDistanceCanvas  => _fraungoferDistanceCanvas;
    public PlayerController PlayerController => _playerController;
    public SimpleWorldPointer Pointer => _pointer;
    

    private GameObject _blocker;
    private PlayerInteractor _playerInteractor;
    private InfoCanvas _infoCanvas;
    private Canvas _fraungoferDistanceCanvas;

    private Computer _computer;

    public Computer Computer => _computer;

    private void OnDisable()
    {
        if(_playerInteractor == null && _infoCanvas == null)
            return;
        
        _playerInteractor.OnInteract -= _infoCanvas.DisplayInfo;
        _playerInteractor.OnInteractEnd -= _infoCanvas.HideInfo;
    }

    public void InitializeComputer(Computer computer)
    {
        _computer = computer;
    }

    public void InitializeBlocker(GameObject blocker)
    {
        _blocker = blocker;
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
}