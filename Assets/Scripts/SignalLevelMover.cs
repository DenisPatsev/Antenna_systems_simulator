using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SignalLevelMover : MonoBehaviour
{
    [SerializeField] private RectTransform _indicatorsRoot;
    [SerializeField] private RectTransform _indicatorPointprefab;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private TMP_Text _maxdBLabel;
    [SerializeField] private TMP_Text _mindBLabel;
    [FormerlySerializedAs("_minMHzLabel")] [SerializeField] private TMP_Text _minGHzLabel;
    [FormerlySerializedAs("_maxMHzLabel")] [SerializeField] private TMP_Text _maxGHzLabel;
    [SerializeField] private Color _lineColor;
    [Range(0, 300)] public int _pointsCount;
    [Range(0, 0.5f)] public float _changingSpeed;
    [Range(-0.1f, 0.1f)] public float _maxRandomValue;
    [Range(0, 1f)] public float _axisYpos;

    private List<RectTransform> _indicators;
    private RectTransform _mainIndicator;
    private float _randomValue;
    private float _mainLevel;
    private float _maxPosY;
    private float _maxdBValue;
    private float _mindBValue;
    private float _currentGHz;
    private float _minGHz;
    private float _maxGHz;
    private float _maxMainSliderValue;
    private float _minMainSliderValue;
    private int _mainIndicatorIndex;

    private float _timer;

    private void OnEnable()
    {
        _maxPosY = 0.156f;

        _timer = 0;
        _minGHz = 1;
        _maxGHz = 5;
        InitializeLineRenderer();
    }

    private void Update()
    {
        if (_lineRenderer.positionCount <= 0 || _mainIndicator == null)
            return;

        if (_timer >= _changingSpeed)
            RecalculatePointsPosition();
        else
            _timer += Time.deltaTime;

        _mainIndicator.transform.localPosition = new Vector3(_mainIndicator.transform.localPosition.x, _mainLevel, 0);
        // _indicatorsRoot.pivot = new Vector2(_mainIndicator.pivot.x, _axisYpos);
    }

    private void RecalculatePointsPosition()
    {
        for (int i = 0; i < _indicators.Count; i++)
        {
            _randomValue = Random.Range(-_maxRandomValue, _maxRandomValue);

            if (i != _mainIndicatorIndex)
                _indicators[i].localPosition = new Vector3(_indicators[i].localPosition.x, _randomValue, 0);

            _lineRenderer.SetPosition(i,
                new Vector3(_indicators[i].transform.localPosition.x, _indicators[i].transform.localPosition.y, 0));
        }

        _timer = 0;
    }

    private void InitializeLineRenderer()
    {
        _indicators = new List<RectTransform>();

        for (int i = 0; i < _pointsCount; i++)
        {
            RectTransform rectTransform = Instantiate(_indicatorPointprefab, _indicatorsRoot.transform);
            rectTransform.transform.SetParent(_indicatorsRoot.transform);
            _indicators.Add(rectTransform);
        }

        _lineRenderer.startColor = _lineColor;
        _lineRenderer.endColor = _lineColor;
        _lineRenderer.positionCount = _indicators.Count;

        for (int i = 0; i < _indicators.Count; i++)
        {
            _lineRenderer.SetPosition(i,
                new Vector3(_indicators[i].localPosition.x, _indicators[i].localPosition.y, 0));
        }

        SetMainIndicator();
    }

    private void SetMainIndicator()
    {
        Canvas.ForceUpdateCanvases();

        float maxXPos = (_indicators[_pointsCount - 1].transform.localPosition.x -
                         _indicators[0].transform.localPosition.x);
        float mainIndicatorPosX = maxXPos * ((_currentGHz - _minGHz) / (_maxGHz - _minGHz));
        float indicatorsPosXDelta = maxXPos / _indicators.Count;

        for (int i = 0; i < _indicators.Count; i++)
        {
            if (_indicators[i].transform.localPosition.x < mainIndicatorPosX + indicatorsPosXDelta &&
                _indicators[i].transform.localPosition.x > mainIndicatorPosX - indicatorsPosXDelta)
            {
                _mainIndicatorIndex = i;
                Debug.Log(i);
            }
        }

        _mainIndicator = _indicators[_mainIndicatorIndex];
    }

    public void Initialize(float currentMHz)
    {
        _minGHz = 1;
        _maxGHz = 5;
        _mindBValue = -20;
        _maxdBValue = 15;
        _currentGHz = currentMHz;

        _mindBLabel.text = _mindBValue.ToString();
        _minGHzLabel.text = _minGHz.ToString();
        _maxGHzLabel.text = _maxGHz.ToString();
        _maxdBLabel.text = _maxdBValue.ToString();
        
        Debug.Log(_currentGHz);
    }


    public void SetMainIndicatorValue(float value)
    {
        float yAxisLength = Mathf.Abs(_maxdBValue - _mindBValue);
        float currentPosY = _maxPosY * Mathf.Abs((value - _mindBValue)/yAxisLength);
        Debug.Log(_mainLevel);
        _mainLevel = currentPosY;
    }
}