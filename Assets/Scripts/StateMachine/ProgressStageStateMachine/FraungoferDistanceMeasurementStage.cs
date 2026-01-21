using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class FraungoferDistanceMeasurementStage : IProgressStage
{
    private UIEventsService _uiEventsService;
    private GameBootstrapper _gameBootstrapper;
    private TextField _distanceInputField;
    private TextField _lambdaInputField;
    private VisualElement _fraungoferScreen;
    private VisualElement _mainOverlay;
    private VisualElement _root;
    private Label _resultLabel;
    private Button _resultButton;
    private Coroutine _coroutine;

    public FraungoferDistanceMeasurementStage(UIEventsService uiEventsService, GameBootstrapper gameBootstrapper)
    {
        _uiEventsService = uiEventsService;
        _gameBootstrapper = gameBootstrapper;
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
        _uiEventsService.InitializeHintLabel();
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
            if (_distanceInputField.text == null || _distanceInputField.text == "" || _lambdaInputField.text == null ||
                _lambdaInputField.text == "")
                return;

            float distance = float.Parse(_distanceInputField.text);
            float lambda = float.Parse(_lambdaInputField.text);

            float result = (2*distance*distance) / lambda;

            _resultLabel.text = result.ToString("F2");

            if (distance == _gameBootstrapper.SelectedAntenna.aperture &&
                lambda == _gameBootstrapper.SelectedAntenna.wavelength)
            {
                _resultLabel.style.color = Color.green;
                _resultLabel.text = _gameBootstrapper.SelectedAntenna.fraungoferDistance.ToString("F3");

                if (_coroutine != null && _uiEventsService != null)
                    _uiEventsService.StopCoroutine(_coroutine);

                if (_uiEventsService != null)
                    _coroutine = _uiEventsService.StartCoroutine(WaitForEnd());
                Debug.Log("Следующий этап");
            }
            else
            {
                _resultLabel.style.color = Color.red;
                Debug.Log("неверно введено расстояние");
            }
        }
    }

    private IEnumerator WaitForEnd()
    {
        yield return new WaitForSeconds(1.3f);
        EndStage();
    }

    public void EndStage()
    {
        Unsubscribe();
        
        if (_coroutine != null && _uiEventsService != null)
            _uiEventsService.StopCoroutine(_coroutine);
        
        _fraungoferScreen.style.display = DisplayStyle.None;
        _mainOverlay.style.display = DisplayStyle.Flex;
        _uiEventsService.Blocker.gameObject.SetActive(false);
        _uiEventsService.FraungoferDistanceCanvas.gameObject.SetActive(true);
        _uiEventsService.PlayerController.enabled = true;

        CursorStateChanger.DisableCursor();
        OnStageFinished?.Invoke();
    }
}