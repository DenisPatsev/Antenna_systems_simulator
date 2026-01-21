using InteractableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class DiagramsDemoStage : IProgressStage
{
    private ProgressStageStateMachine _progressStageStateMachine;
    private StageObjectsData _stageData;
    private Computer _computer;
    private UIEventsService _uiEventsService;
    private GameBootstrapper _gameBootstrapper;
    
    private VisualElement _cursor;
    private VisualElement _root;
    private Label _hint;
    private StageObjectsData _stageObjectsData;
    private VisualElement _hintPlace;
    private Coroutine _hintAnimationCoroutine;

    public UnityAction OnStageStarted { get; set; }
    public UnityAction OnStageFinished { get; set; }

    public DiagramsDemoStage(ProgressStageStateMachine progressStageStateMachine, GameBootstrapper gameBootstrapper)
    {
        _progressStageStateMachine = progressStageStateMachine;
        _gameBootstrapper = gameBootstrapper;
        _uiEventsService = _gameBootstrapper.UIEventsService;
    }
    
    public void StartStage()
    {
        Subscribe();
        _stageData = _progressStageStateMachine.GetDataByName(Constants.DiagramViewStageID);
        _computer = _stageData.GetStageObjectByType(typeof(Computer)) as Computer;

        _computer.SetInteractableState();
        _computer.OnInteract += EndStage;
        
        _stageObjectsData = _gameBootstrapper.ProgressStageStateMachine.GetDataByName(Constants.DiagramViewStageID);
       
        _hint.text = _stageObjectsData.Hint;
        _hintAnimationCoroutine = _uiEventsService.StartCoroutine(UIElementsAnimationService.ShuffleTextCoroutine(_hint, _stageObjectsData.Hint, Constants.HintAnimationSpeed));
        // _uiEventsService.StartCoroutine(UIElementsAnimationService.PingPongScale(_hintPlace, 1.15f, 0.3f, 3));
        // _uiEventsService.Pointer.SetTargetObject(_computer.transform);
    }
    
    private void Subscribe()
    {
        _uiEventsService.GameLoopScreen.gameObject.SetActive(true);
        _root = _uiEventsService.GameLoopScreen.rootVisualElement;
        _hint = _root.Q<Label>("Hint");
        _hintPlace = _root.Q<VisualElement>("HintPlace");
        _cursor = _root.Q<VisualElement>("Cursor");
    }

    public void EndStage()
    {
        if(_hintAnimationCoroutine != null)
            _uiEventsService.StopCoroutine(_hintAnimationCoroutine);
            
        _computer.ShowDiagram(_gameBootstrapper.SelectedAntenna.diagram2DImage);
        _computer.OnInteract += EndStage;
        OnStageFinished?.Invoke();
    }
}