using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AttentionBlockerController : MonoBehaviour
{
   public InteractableUIButton _acceptionButton;
   public GameObject _blocker;

   public UnityAction OnAccepted;

   private void OnEnable()
   {
      _acceptionButton.OnInteract += Accept;
   }

   private void Accept()
   {
      OnAccepted?.Invoke();
      _blocker.SetActive(false);
   }
}
