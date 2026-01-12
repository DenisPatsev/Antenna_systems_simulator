using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class FraungoferDistanceMeasurementStage : IProgressStage
{
    private UIEventsService _uiEventsService;
    private TextField _distanceInputField;
    private TextField _lambdaInputField;
    private VisualElement _fraungoferScreen;
    private VisualElement _mainOverlay;
    private VisualElement _root;
    private Label _resultLabel;
    private Button _resultButton;
    
    public FraungoferDistanceMeasurementStage(UIEventsService uiEventsService)
    {
        _uiEventsService = uiEventsService;
    }

    public UnityAction OnStageStarted { get; set; }
    public UnityAction OnStageFinished { get; set; }

    public void StartStage()
    {
        _uiEventsService.GameLoopScreen.gameObject.SetActive(true);
        _uiEventsService.PlayerController.enabled = false;
        _root = _uiEventsService.GameLoopScreen.rootVisualElement;
        _fraungoferScreen = _root.Q<VisualElement>("FraungofferDistanceOverlay");
        _fraungoferScreen.style.display = DisplayStyle.Flex;
        CursorStateChanger.EnableCursor();
        Subscribe();
    }

    private void Subscribe()
    {
        _resultButton = _root.Q<Button>("ResultButton");
        _distanceInputField = _root.Q<TextField>("Size");
        _lambdaInputField = _root.Q<TextField>("WaveLength");
        _resultLabel = _root.Q<Label>("Result");
        _mainOverlay = _root.Q<VisualElement>("MainOverlay");
        
        _mainOverlay.style.display = DisplayStyle.None;
        
        _resultButton.clicked += OnResultButtonClicked;
    }

    private void Unsubscribe()
    {
        _resultButton.clicked -= OnResultButtonClicked;
    }
    
    private void OnResultButtonClicked()
    {
        if (_distanceInputField.text != null && _lambdaInputField.text != null)
        {
            float distance = float.Parse(_distanceInputField.text);
            float lambda = float.Parse(_lambdaInputField.text);
            
            float result = distance/lambda;
            
            _resultLabel.text = result.ToString("F2");

            if (result != (float)(5/1))
            {
                _resultLabel.style.color = Color.red;
                Debug.Log("неверно введено расстояние");
            }
            else
            {
                _resultLabel.style.color = Color.green;
                EndStage();
                Debug.Log("Следующий этап");
            }
            
            // OnStageComplete?.Invoke();
        }
            
    }

    public void EndStage()
    {
       Unsubscribe();
       _fraungoferScreen.style.display = DisplayStyle.None;
       _mainOverlay.style.display = DisplayStyle.Flex;
       _uiEventsService.Blocker.gameObject.SetActive(false);
       _uiEventsService.FraungoferDistanceCanvas.gameObject.SetActive(true);
       _uiEventsService.PlayerController.enabled = true;
       
       CursorStateChanger.DisableCursor();
       OnStageFinished?.Invoke();
    }
}