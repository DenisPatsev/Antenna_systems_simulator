using UnityEngine.Events;

public interface IProgressStage
{
    UnityAction OnStageStarted{get;set;}
    UnityAction OnStageFinished{get;set;}
    void StartStage();
    void EndStage();
}