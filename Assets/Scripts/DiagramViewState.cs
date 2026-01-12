using UnityEngine;
using UnityEngine.UIElements;

public class DiagramViewState : IState
{
    private VisualElement _root;
    private UIEventsService _uiEventsService;
    private SceneLoader _sceneLoader;
    private StateMachine _stateMachine;
    private DiagramBuilder _diagramBuilder;

    private Button _exitButton;
    private Button _showAllButton;
    private Label _degrees;
    private VisualElement _arrowContainer;

    private bool _isAllAngles;

    public DiagramViewState(GameBootstrapper bootstrapper, StateMachine stateMachine)
    {
        _uiEventsService = bootstrapper.UIEventsService;
        _sceneLoader = bootstrapper.SceneLoader;
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _uiEventsService.DiagramScreen.gameObject.SetActive(true);
        _root = _uiEventsService.DiagramScreen.rootVisualElement;

        CreateDiagram();

        Subscribe();
    }

    public void Subscribe()
    {
        _exitButton = _root.Q<Button>("ExitButton");
        _degrees = _root.Q<Label>("Degrees");
        _arrowContainer = _root.Q<VisualElement>("ArrowContainer");
        _showAllButton = _root.Q<Button>("ShowAllButton");

        _degrees.text = PlayerSessionData.CurrentAntennaRotationZ.ToString() + " deg.";
        _arrowContainer.style.rotate =
            new StyleRotate(Quaternion.Euler(0, 0, PlayerSessionData.CurrentAntennaRotationZ));

        _exitButton.clicked += OnExitButtonClicked;
        _showAllButton.clicked += OnShowAllButtonClicked;
    }

    private void CreateDiagram()
    {
        GameObject diagramPrefab =
            GameFactory.CreateObject(Constants.DiagramViewPath, new Vector3(1,1,1), Quaternion.identity);
        
        diagramPrefab.transform.parent = _uiEventsService.transform;

        _diagramBuilder = diagramPrefab.GetComponentInChildren<DiagramBuilder>();
    }

    private void OnExitButtonClicked()
    {
        Debug.Log("Exit");
        _sceneLoader.LoadScene(Constants.MainSceneName, OnLoadScene);
    }

    private void OnShowAllButtonClicked()
    {
        if (!_isAllAngles)
        {
            _showAllButton.style.backgroundColor = Color.green;
            _diagramBuilder.showAllAngles = true;
            _diagramBuilder.GenerateMeshFromCurrentSelection();
            _degrees.text = "3D mode";
            _isAllAngles = true;
        }
        else
        {
            _showAllButton.style.backgroundColor = new Color(0.8f, 0.8f, 0f);
            _diagramBuilder.showAllAngles = false;
            _diagramBuilder.GenerateMeshFromCurrentSelection();
            _degrees.text = PlayerSessionData.CurrentAntennaRotationZ.ToString() + " deg.";
            _isAllAngles = false;
        }
    }

    private void OnLoadScene()
    {
        _stateMachine.Enter<GameLoopState>();
    }

    public void Unsubscribe()
    {
        _exitButton.clicked -= OnExitButtonClicked;
        _showAllButton.clicked -= OnShowAllButtonClicked;
    }

    public void Exit()
    {
        Unsubscribe();
        GameObject.Destroy(_diagramBuilder.gameObject.transform.parent.gameObject);
        _uiEventsService.DiagramScreen.gameObject.SetActive(false);
    }
}