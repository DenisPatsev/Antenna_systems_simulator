using UnityEngine.SceneManagement;

public class Door : InteractableObject
{
    public override void Interact()
    {
        CursorStateChanger.EnableCursor();
        SceneManager.LoadScene(Constants.InitialSceneName);
    }
}