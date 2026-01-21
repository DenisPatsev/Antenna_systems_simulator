using InteractableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class VectorAnalyzerEnablingStage : IProgressStage
{
    private ProgressStageStateMachine _stateMachine;
    private StageObjectsData _currentStage;
    private VectorAnalyzer _analyzer;
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

    public VectorAnalyzerEnablingStage(ProgressStageStateMachine stateMachine, GameBootstrapper gameBootstrapper)
    {
        _stateMachine = stateMachine;
        _gameBootstrapper = gameBootstrapper;
        _uiEventsService = gameBootstrapper.UIEventsService;
    }
    
    public void StartStage()
    {
        Subscribe();
        _currentStage = _stateMachine.GetDataByName(Constants.VectorAnalyzerStageDataID);
        _analyzer = _currentStage.GetStageObjectByType(typeof (VectorAnalyzer)) as VectorAnalyzer;

        _analyzer.OnActivate += EndStage;
        
        _stageObjectsData = _gameBootstrapper.ProgressStageStateMachine.GetDataByName(Constants.VectorAnalyzerStageDataID);
       
        _uiEventsService.Pointer.enabled = true;
        _uiEventsService.Pointer.SetTargetObject(_analyzer.transform);
        _hint.text = _stageObjectsData.Hint;
        _hintAnimationCoroutine = _uiEventsService.StartCoroutine(UIElementsAnimationService.ShuffleTextCoroutine(_hint, _stageObjectsData.Hint, Constants.HintAnimationSpeed));
       // _hintAnimationCoroutine = _uiEventsService.StartCoroutine(UIElementsAnimationService.PingPongScale(_hintPlace, 1.15f, 0.3f, 3));
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
        if(_hintAnimationCoroutine != null && _uiEventsService != null)
            _uiEventsService.StopCoroutine(_hintAnimationCoroutine);
        
        _uiEventsService.Pointer.enabled = false;
        _analyzer.OnActivate -= EndStage;
        OnStageFinished?.Invoke();
    }
}