using InteractableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class GeneratorEnablingStage : IProgressStage
{
    private ProgressStageStateMachine _progressStageStateMachine;
    private StageObjectsData _stageData;
    private GameBootstrapper _gameBootstrapper;
    private UIEventsService _uiEventsService;
    private Generator _generator;

    private Label _hint;
    private VisualElement _root;
    private VisualElement _cursor;
    private StageObjectsData _stageObjectsData;
    private VisualElement _hintPlace;
    private Coroutine _hintAnimationCoroutine;

    public UnityAction OnStageStarted { get; set; }
    public UnityAction OnStageFinished { get; set; }
    
    public GeneratorEnablingStage(ProgressStageStateMachine stateMachine, GameBootstrapper gameBootstrapper)
    {
        _progressStageStateMachine = stateMachine;
        _gameBootstrapper = gameBootstrapper;
        _uiEventsService = _gameBootstrapper.UIEventsService;
    }
    
    public void StartStage()
    {
        Subscribe();
        _stageData = _progressStageStateMachine.GetDataByName(Constants.GeneratorActivationStageDataFileID);
        _generator = _stageData.GetStageObjectByType(typeof(Generator)) as Generator;

        _generator.OnActivate += EndStage;
        
        _stageObjectsData = _gameBootstrapper.ProgressStageStateMachine.GetDataByName(Constants.GeneratorActivationStageDataFileID);
       
        _uiEventsService.Pointer.enabled = true;
        _uiEventsService.Pointer.SetTargetObject(_generator.transform);
        _hint.text = _stageObjectsData.Hint;
        _hintAnimationCoroutine = _uiEventsService.StartCoroutine(UIElementsAnimationService.ShuffleTextCoroutine(_hint, _stageObjectsData.Hint, 750f));
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
        if(_hintAnimationCoroutine != null)
            _uiEventsService.StopCoroutine(_hintAnimationCoroutine);
        
        _uiEventsService.Pointer.enabled = false;
        _generator.OnActivate -= EndStage;
        OnStageFinished?.Invoke();
    }
}