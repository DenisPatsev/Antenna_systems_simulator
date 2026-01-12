using UnityEngine;

public class ProgressStageStateMachine : MonoBehaviour
{
    [SerializeField] private UIEventsService _uiEventsService;
    [SerializeField] private GameBootstrapper _gameBootstrapper;
    [SerializeField] private StageObjectsData[] _stageObjectsData;

    private IProgressStage[] _progressStages;
    private int _currentStageIndex;

    public StageObjectsData[] StageObjectsData => _stageObjectsData;

    private void OnEnable()
    {
        _progressStages = new IProgressStage[7];

        _progressStages[0] = new FraungoferDistanceMeasurementStage(_uiEventsService);
        _progressStages[1] = new AttentionStage(_gameBootstrapper.UIEventsService, _gameBootstrapper);
        _progressStages[2] = new GeneratorEnablingStage(this, _gameBootstrapper);
        _progressStages[3] = new VectorAnalyzerEnablingStage(this, _gameBootstrapper);
        _progressStages[4] = new FrequencySetupStage(this, _gameBootstrapper);
        _progressStages[5] = new MeasurementStage(this, _gameBootstrapper);
        _progressStages[6] = new DiagramsDemoStage(this, _gameBootstrapper);

        _currentStageIndex = 0;
        Subscribe();
        _progressStages[_currentStageIndex].StartStage();
    }

    private void SwitchToNextStage()
    {
        Unsubscribe();
        Debug.Log("Stage finished - " + _progressStages[_currentStageIndex]);
        _currentStageIndex++;

        if (_currentStageIndex >= _progressStages.Length)
        {
            Debug.Log("Not enough stages.");
            return;
        }

        _progressStages[_currentStageIndex].StartStage();
        Debug.Log("Stage changed");

        Subscribe();
    }

    public StageObjectsData GetDataByName(string dataName)
    {
        foreach (var data in _stageObjectsData)
        {
            if (data.StageName == dataName)
            {
                return data;
            }
        }

        return null;
    }

    private void Subscribe()
    {
        _progressStages[_currentStageIndex].OnStageFinished += SwitchToNextStage;
    }

    private void Unsubscribe()
    {
        _progressStages[_currentStageIndex].OnStageFinished -= SwitchToNextStage;
    }
}