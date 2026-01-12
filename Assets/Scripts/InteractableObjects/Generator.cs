using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace InteractableObjects
{
    public class Generator : InteractableObject
    {
        public GameObject LoadingPanel;
        public GameObject MainInfoPanel;
        public GameObject InactivePanel;
        public TMP_Text ErrorMessage;
        public Button ApplyButton;
        public TMP_InputField InputField;
        public float LoadingDuration;
        public float ErrorMessageDuration;

        private bool _isEnabled;
        private Coroutine _starterCoroutine;
        private Coroutine _errorCoroutine;
        private float _targetFrequency;
        private string _defaultInfo;
        private string _errorInfo;
        private Color _defaultColor;
        private Color _errorColor;
        
        public event UnityAction OnActivate;
        public event UnityAction OnFrequencySetted;

        private void OnEnable()
        {
            base.OnEnable();
            
            _isEnabled = false;
            _defaultInfo = ErrorMessage.text;
            _errorInfo = "Неверное значение частоты";
            _defaultColor = ErrorMessage.color;
            _errorColor = Color.red;
            
            ApplyButton.onClick.AddListener(TryApplyFrequency);
        }

        private void OnDisable()
        {
            base.OnDisable();
            ApplyButton.onClick.RemoveListener(TryApplyFrequency);
        }

        private void TryApplyFrequency()
        {
            if(InputField.text == _targetFrequency.ToString())
                OnFrequencySetted?.Invoke();
            else
                ShowErrorMessage();
        }

        private void Activate()
        {
            if(_starterCoroutine != null)
                StopCoroutine(_starterCoroutine);

            _starterCoroutine = StartCoroutine(Starter());
            OnActivate?.Invoke();
        }

        private IEnumerator Starter()
        {
            LoadingPanel.SetActive(true);
            yield return new WaitForSeconds(LoadingDuration);
            
            LoadingPanel.SetActive(false);
            InactivePanel.SetActive(false);
            MainInfoPanel.SetActive(true);
            _isEnabled = true;
            _focusableObject.enabled = true;
        }

        private void ShowErrorMessage()
        {
            if(_errorCoroutine != null)
                StopCoroutine(_errorCoroutine);

            _errorCoroutine = StartCoroutine(ErrorShower());
        }

        private IEnumerator ErrorShower()
        {
            ErrorMessage.text = _errorInfo;
            ErrorMessage.color = _errorColor;
            
            yield return new WaitForSeconds(ErrorMessageDuration);
            
            ErrorMessage.text = _defaultInfo;
            ErrorMessage.color = _defaultColor;
        }

        public void SetTargetFrequency(float frequency)
        {
            _targetFrequency = frequency;
        }
        
        public override void Interact()
        {
            if (!_isEnabled)
            {
                Activate();
                return;
            }
            
            _uiEventsService.GameLoopScreen.gameObject.SetActive(false);
        }
        
    }
}