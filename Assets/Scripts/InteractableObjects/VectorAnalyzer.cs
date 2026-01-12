using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace InteractableObjects
{
    public class VectorAnalyzer : InteractableObject
    {
        [SerializeField] private GameObject _inactivePanel;
        [SerializeField] private GameObject _mainInfo;
        [SerializeField] private GameObject _loadingPanel;
        [SerializeField] private float _loadingDuration;
        [SerializeField] private SignalLevelMover _signalLevelMover;

        private bool _isEnabled;
        private Coroutine _loadingCoroutine;

        public UnityAction OnActivate;

        public SignalLevelMover SignalLevelMover => _signalLevelMover;

        private void OnEnable()
        {
            base.OnEnable();

            _isEnabled = false;
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

        private void Activate()
        {
            if (_loadingCoroutine != null)
                StopCoroutine(_loadingCoroutine);

            _loadingCoroutine = StartCoroutine(Starter());
            OnActivate?.Invoke();
        }

        private IEnumerator Starter()
        {
            _loadingPanel.SetActive(true);
            yield return new WaitForSeconds(_loadingDuration);

            _loadingPanel.SetActive(false);
            _inactivePanel.SetActive(false);
            _mainInfo.SetActive(true);
            _focusableObject.enabled = true;
            _isEnabled = true;
        }

        public void EnableMeasurementMode(float currentMHz)
        {
            _signalLevelMover.Initialize(currentMHz);
            _signalLevelMover.enabled = true;
        }
    }
}