using System;
using System.Collections;
using Interfaces;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour, ICoroutineRunner
{
    [SerializeField] private UIEventsService _uiEventsService;
    [SerializeField] private ProgressStageStateMachine _progressStageStateMachine;
    [SerializeField] private AntennaData[] _antennaDatas;

    private SceneLoader _sceneLoader;
    private StateMachine _stateMachine;
    private Coroutine _enablingCoroutine;
    private AntennaData _selectedAntenna;

    public SceneLoader SceneLoader => _sceneLoader;
    public UIEventsService UIEventsService => _uiEventsService;
    public ProgressStageStateMachine ProgressStageStateMachine => _progressStageStateMachine;
    public AntennaData[] AntennaDatas => _antennaDatas;
    public AntennaData SelectedAntenna => _selectedAntenna;

    private void Awake()
    {
        _sceneLoader = new SceneLoader(this);
        _stateMachine = new StateMachine(this);

        _stateMachine.Enter<InitialState>();
    }

    public void SetCurrentAntennaData(string antennaName)
    {
        foreach (var data in _antennaDatas)
        {
            if(data.antennaName == antennaName)
                _selectedAntenna = data;
        }

        if (_selectedAntenna != null)
        {
            _selectedAntenna.signalLevelValues = DataParserStatic.GetDataFromFile(_selectedAntenna.diagram2DDatafile);
            Debug.Log("AntennaData successfully parsed. First value - " + _selectedAntenna.signalLevelValues[0]);
        }
        
    }

    public void EnableStateMachineDelayed(float delay, ProgressStageStateMachine progressStage)
    {
        if(_enablingCoroutine != null)
            StopCoroutine(_enablingCoroutine);
        
        _enablingCoroutine = StartCoroutine(DelayedObjectsEnabler(delay, progressStage));
    }

    public void AddStageData(string stageName, IStageObject stageObject)
    {
        foreach (var data in _progressStageStateMachine.StageObjectsData)
        {
            if(data.StageName == stageName)
                data.stageObjects.Add(stageObject);
        }
    }

    private IEnumerator DelayedObjectsEnabler(float delay, ProgressStageStateMachine progressStage)
    {
        yield return new WaitForSeconds(delay);
        
        progressStage.enabled = true;
    }
}