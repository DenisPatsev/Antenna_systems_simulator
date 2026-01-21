using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class AttentionStage : IProgressStage
{
    private AttentionBlockerController _attentionBlockerController;
    private UIEventsService _uiEventsService;
    private GameBootstrapper _gameBootstrapper;
    private StageObjectsData _stageObjectsData;

    private Label _hint;
    private VisualElement _cursor;
    private VisualElement _root;
    private VisualElement _hintPlace;
    private Coroutine _hintAnimationCoroutine;

    public UnityAction OnStageStarted { get; set; }
    public UnityAction OnStageFinished { get; set; }

    public AttentionStage(UIEventsService uiEventsService, GameBootstrapper gameBootstrapper)
    {
        _uiEventsService = uiEventsService;
        _gameBootstrapper = gameBootstrapper;
    }

    public void StartStage()
    {
        Subscribe();

        GameObject blocker = GameFactory.CreateObject(Constants.AttentionBlockerPath,
            Constants.AttentionBlockerPosition, Quaternion.Euler(Constants.AttentionBlockerRotation));
        _attentionBlockerController = blocker.GetComponent<AttentionBlockerController>();
        _attentionBlockerController.OnAccepted += EndStage;

        _stageObjectsData =
            _gameBootstrapper.ProgressStageStateMachine.GetDataByName(Constants.AttentionStageDataFileID);

        _hint.text = _stageObjectsData.Hint;
        _hintAnimationCoroutine =
            _uiEventsService.StartCoroutine(
                UIElementsAnimationService.ShuffleTextCoroutine(_hint, _stageObjectsData.Hint, Constants.HintAnimationSpeed));
        _uiEventsService.Pointer.SetTargetObject(blocker.transform);
        _uiEventsService.Pointer.enabled = true;
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
        if (_hintAnimationCoroutine != null && _uiEventsService != null)
            _uiEventsService.StopCoroutine(_hintAnimationCoroutine);

        if (_uiEventsService.Pointer != null)
            _uiEventsService.Pointer.enabled = false;
        
        OnStageFinished?.Invoke();
        _attentionBlockerController.OnAccepted -= EndStage;
    }
}