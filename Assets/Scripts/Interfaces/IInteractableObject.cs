using Interfaces;

public interface IInteractableObject : IStageObject
{
    void SetOutline();
    void SetDefaultMaterial();
    void Interact();
}