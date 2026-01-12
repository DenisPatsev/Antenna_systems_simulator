using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class TextShuffleEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private TextItem[] _textItems;
    [SerializeField] private float _shuffleSpeed;
    private TextShuffleEffect instance;
    private char[] _symbols;
    
    private Coroutine _coroutine;
    private int _currentIndex;

    private void Start()
    {
        instance = this;
        _currentIndex = 0;
        _symbols = new char[]
        {
            'A', 'B', 'C', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'V', 'W',
            'X', 'Y', 'Z', '#', '@', '!', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '/'
        };

        ShuffleText(_textItems[_currentIndex].text, _shuffleSpeed);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _currentIndex ++;
            
            if(_currentIndex >= _textItems.Length)
                _currentIndex = 0;
            
            ShuffleText(_textItems[_currentIndex].text, _shuffleSpeed);
        }
    }

    public void ShuffleText(string text, float shuffleSpeed)
    {
        if(_coroutine != null)
            StopCoroutine(_coroutine);
        
        // _coroutine = StartCoroutine(ShuffleTextCoroutine(text, shuffleSpeed));
    }

    private IEnumerator ShuffleTextCoroutine(Label label,string text, float shuffleSpeed)
    {
        float timer = 0f;
        char[] tempArray = new char[text.Length];
        label.text = "";

        for (int i = 0; i < text.Length; i++)
        {
            tempArray[i] = _symbols[Random.Range(0, _symbols.Length)];

            for (int j = 0; j < _symbols.Length; j++)
            {
                tempArray[i] = _symbols[Random.Range(0, _symbols.Length)];
                _text.text = tempArray.ArrayToString();
                yield return new WaitForSeconds(1/shuffleSpeed);
            }

            tempArray[i] = text[i];
            label.text = tempArray.ArrayToString();
        }
    }
}

[Serializable]
public class TextItem
{
    public string text;
}