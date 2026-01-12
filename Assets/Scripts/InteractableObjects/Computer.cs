using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace InteractableObjects
{
    public class Computer : InteractableObject
    {
        [SerializeField] protected RawImage _image;
        public event UnityAction OnInteract;

        private bool _canInteract;

        protected void OnEnable()
        {
            base.OnEnable();
            
            _canInteract = false;
        }
        
        public override void Interact()
        {
            if(!_canInteract)
                return;
            
            OnInteract?.Invoke();
            Debug.Log("Interact");
        }

        public void ShowDiagram(Texture2D texture)
        {
            _image.texture = texture;
            _image.color = new Color(1, 1, 1, 1);
        }

        public void Initialize(UIEventsService eventsService)
        {
            _uiEventsService = eventsService;
        }

        public void SetInteractableState()
        {
            _canInteract = true;
        }
    }
}