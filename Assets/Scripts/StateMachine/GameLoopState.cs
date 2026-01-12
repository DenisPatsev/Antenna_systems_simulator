using InteractableObjects;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameLoopState : IState
{
    private SceneLoader _sceneLoader;
    private UIEventsService _uiEventsService;
    private Computer _computer;
    private GameBootstrapper _gameBootstrapper;
    private StateMachine _stateMachine;
    private PlayerInteractor _playerInteractor;
    private InfoCanvas _infoCanvas;
    private GameObject _blocker;
    private DiagramBuilder _diagramBuilder;
    private Transform _postProcessVolume;

    public GameLoopState(GameBootstrapper gameBootstrapper, StateMachine stateMachine)
    {
        _sceneLoader = gameBootstrapper.SceneLoader;
        _gameBootstrapper = gameBootstrapper;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _uiEventsService = _gameBootstrapper.UIEventsService;
        
        CreateGameWorld();
        
        _uiEventsService.InitializeComputer(_computer);
        _uiEventsService.InitializeMainSceneObjects(_playerInteractor, _infoCanvas);
        _computer = _uiEventsService.Computer;
        _gameBootstrapper.EnableStateMachineDelayed(2f, _gameBootstrapper.ProgressStageStateMachine);
        
        CursorStateChanger.DisableCursor();

        Subscribe();
    }

    public void Subscribe()
    {
        _computer.OnInteract += OnComputerInteraction;
    }

    public void Unsubscribe()
    {
        _computer.OnInteract -= OnComputerInteraction;
    }

    private void CreateGameWorld()
    {
        CreatePostProcessingVolume();
        CreatePlayer();
        CreateInfoCanvas();
        CreateComputer();
        CreateBlocker();
        CreateFraungoferDistanceCanvas();
        CreateGenerator();
        CreateVectorAnalyzer();
        CreateMeasurableAntenna();
    }

    private void CreatePlayer()
    {
        GameObject player = GameFactory.CreatePlayer(Constants.PlayerPrefabPath,
            new Vector3(-2.22000003f, 2, -1.92999995f));
        _playerInteractor = player.GetComponentInChildren<PlayerInteractor>();
        PlayerController playerController = player.GetComponentInChildren<PlayerController>();
        _uiEventsService.InitializePlayerController(playerController);
        
        Camera playerCamera = player.GetComponentInChildren<Camera>();
        PostProcessLayer ppLayer = playerCamera.GetComponent<PostProcessLayer>();
        
        ppLayer.volumeTrigger = _postProcessVolume;
    }

    private void CreateInfoCanvas()
    {
        GameObject canvas = GameFactory.CreateInfoCanvas(Constants.InfoCanvasPrefabPath, Vector3.zero);
        _infoCanvas = canvas.GetComponentInChildren<InfoCanvas>();
        _infoCanvas.InitializePlayer(_playerInteractor.gameObject);
    }

    private void CreatePostProcessingVolume()
    {
        GameObject pp = GameFactory.CreateObject(Constants.PostProcessingPrefabPath, Vector3.zero, Quaternion.identity);

        _postProcessVolume = pp.gameObject.transform;
    }

    private void CreateComputer()
    {
        Vector3 computerPosition =  new Vector3(Constants.ComputerPositionX, Constants.ComputerPositionY, Constants.ComputerPositionZ);
        Quaternion computerRotation = Quaternion.Euler(Constants.ComputerRotationX, Constants.ComputerRotationY, Constants.ComputerRotationZ);
        
        GameObject computer = GameFactory.CreateObject(Constants.ComputerPath, computerPosition, computerRotation );
        
        _computer = computer.GetComponent<Computer>();
        _computer.Initialize(_uiEventsService);
        
        _gameBootstrapper.AddStageData(Constants.DiagramViewStageID, _computer);
    }

    private void CreateVectorAnalyzer()
    {
        GameObject vectornik = GameFactory.CreateObject(Constants.VectorAnalyzerPath, Constants.VectorAnalyzerPosition, Quaternion.Euler(Constants.VectorAnalyzerRotation));
        VectorAnalyzer vectorAnalyzer = vectornik.GetComponent<VectorAnalyzer>();
        vectorAnalyzer.Initialize(_uiEventsService);
        
        _gameBootstrapper.AddStageData(Constants.VectorAnalyzerStageDataID, vectorAnalyzer);
    }

    private void CreateFraungoferDistanceCanvas()
    {
        GameObject distanceCanvas = GameFactory.CreateObject(Constants.FraungoferDistanceCanvasPath, Constants.FraungoferDistanceCanvasPosition, Quaternion.Euler(Constants.FraungoferDistanceCanvasRotation));
        Canvas canvas = distanceCanvas.GetComponentInChildren<Canvas>();
        _uiEventsService.InitializeFraungoferDistanceCanvas(canvas);
        _uiEventsService.FraungoferDistanceCanvas.gameObject.SetActive(false);
    }

    private void CreateMeasurableAntenna()
    {
        GameObject antennaPrefab = GameFactory.CreateObject(Constants.MeasurableAntennaPath, Constants.MeasurableAntennaPosition, Quaternion.Euler(Constants.MeasurableAntennaRotation));
        TrenogaWheel wheel = antennaPrefab.GetComponentInChildren<TrenogaWheel>();
        
        GameObject rotator = GameFactory.CreateObject(Constants.AntennaRotatorPath, Constants.AntennaRotatorPosition, Quaternion.Euler(Constants.AntennaRotatorRotation));
        AntennaRotator rotatorScript = rotator.GetComponent<AntennaRotator>();
        rotatorScript.InitializeWheel(wheel.gameObject);
        rotatorScript.gameObject.SetActive(false);
        
        DiagramBuilder diagramBuilder = antennaPrefab.GetComponentInChildren<DiagramBuilder>();
        diagramBuilder.gameObject.SetActive(false);
        _diagramBuilder = diagramBuilder;
        
        _gameBootstrapper.AddStageData(Constants.MeasurementStageID, rotatorScript);
    }

    private void CreateGenerator()
    {
        GameObject generator = GameFactory.CreateObject(Constants.GeneratorPath, Constants.GeneratorPosition, Quaternion.Euler(Constants.GeneratorRotation));
        Generator generatorScript = generator.GetComponent<Generator>();
        generatorScript.Initialize(_uiEventsService);
        
        _gameBootstrapper.AddStageData(Constants.GeneratorActivationStageDataFileID, generatorScript);
    }

    private void CreateBlocker()
    {
        _blocker = GameFactory.CreateObject(Constants.FirstStageBlockerPath, Constants.FirstStageBlockerPosition,
           Quaternion.Euler(Constants.FirstStageBlockerRotation));
        
        _uiEventsService.InitializeBlocker(_blocker);
    }

    private void OnComputerInteraction()
    {
        _diagramBuilder.gameObject.SetActive(true);
    }

    public void Exit()
    {
        Unsubscribe();
    }
}