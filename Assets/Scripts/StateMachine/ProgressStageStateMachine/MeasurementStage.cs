using InteractableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class MeasurementStage : IProgressStage
{
    private ProgressStageStateMachine _progressStageStateMachine;
    private StageObjectsData _stageData;
    private AntennaRotator _rotator;
    private VectorAnalyzer _analyzer;
    private GameBootstrapper _bootstrapper;
    private UIEventsService _uiEventsService;
    
    private VisualElement _cursor;
    private VisualElement _root;
    private Label _hint;
    private StageObjectsData _stageObjectsData;

    private VisualElement _icon;
    private VisualElement _hintPlace;
    private Coroutine _hintAnimationCoroutine;

    public UnityAction OnStageStarted { get; set; }
    public UnityAction OnStageFinished { get; set; }

    public MeasurementStage(ProgressStageStateMachine progressStageStateMachine, GameBootstrapper gameBootstrapper)
    {
        _progressStageStateMachine = progressStageStateMachine;
        _bootstrapper = gameBootstrapper;
        _uiEventsService = _bootstrapper.UIEventsService;
    }

    private void UpdateSignalInfo()
    {
        _rotator.ShowSignalLevel(_bootstrapper.SelectedAntenna.signalLevelValues[(int)_rotator.RotationZ]);
        _analyzer.SignalLevelMover.SetMainIndicatorValue(_bootstrapper.SelectedAntenna.signalLevelValues[(int)_rotator.RotationZ]);
    }
    
    public void StartStage()
    {
        Subscribe();
        _uiEventsService.FraungoferDistanceCanvas.gameObject.SetActive(false);
        _stageData = _progressStageStateMachine.GetDataByName(Constants.MeasurementStageID);
        _rotator = _stageData.GetStageObjectByType(typeof(AntennaRotator)) as AntennaRotator;
        _stageData = _progressStageStateMachine.GetDataByName(Constants.VectorAnalyzerStageDataID);
        _analyzer = _stageData.GetStageObjectByType(typeof(VectorAnalyzer)) as VectorAnalyzer;
        _rotator.gameObject.SetActive(true);
        _rotator.OnInfoUpdated += UpdateSignalInfo;
        _analyzer.EnableMeasurementMode(_bootstrapper.SelectedAntenna.frequencyGHz);
        UpdateSignalInfo();

        _rotator.OnAllAnglesMeasured += EndStage;
        
        _stageObjectsData = _bootstrapper.ProgressStageStateMachine.GetDataByName(Constants.MeasurementStageID);

        _uiEventsService.Pointer.enabled = true;
        _uiEventsService.Pointer.SetTargetObject(_rotator.gameObject.transform);
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
        if(_hintAnimationCoroutine != null && _uiEventsService.GameLoopScreen.gameObject.activeInHierarchy && _uiEventsService.GameLoopScreen != null)
            _uiEventsService.StopCoroutine(_hintAnimationCoroutine);
        
        _rotator.gameObject.SetActive(false);
        
        if(_uiEventsService.Pointer != null)
            _uiEventsService.Pointer.enabled = false;
        
        _cursor.style.display = DisplayStyle.None;
        
        _rotator.OnInfoUpdated -= UpdateSignalInfo;
        _rotator.OnAllAnglesMeasured -= EndStage;
        OnStageFinished?.Invoke();
    }
}