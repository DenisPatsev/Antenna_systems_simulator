using UnityEngine.Events;

public class Door : InteractableObject
{
    public UnityAction OnGameExit;
        
    public override void Interact()
    {
        CursorStateChanger.EnableCursor();
        OnGameExit?.Invoke();
    }
}