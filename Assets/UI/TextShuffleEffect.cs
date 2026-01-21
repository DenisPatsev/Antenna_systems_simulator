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
            'A', 'B', 'C', 'E', 'F', 'G', 'I', 'K', 'M', 'N', 'O', 'R', 'S', 'T', 'U', 'V', 'W',
            'X', 'Y', 'Z', '#', '@', '!', '%', '^', '&', '*', '(', ')', '-', '+', '=', '/'
        };

        ShuffleText(_textItems[_currentIndex].text, _shuffleSpeed);
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
        WaitForSeconds wait = new WaitForSeconds(1/shuffleSpeed);
        int randomLenght = Random.Range(0, 4);

        for (int i = 0; i < text.Length; i++)
        {
            // tempArray[i] = _symbols[20];
            //
            // for (int j = 0; j < randomLenght; j++)
            // {
            //     tempArray[i] = _symbols[Random.Range(0, _symbols.Length)];
            //     _text.text = tempArray.ArrayToString();
            //     yield return wait;
            // }
            //
            // tempArray[i] = text[i];
            // label.text = tempArray.ArrayToString();
            label.text += text[i];
            yield return wait;
        }
    }
}

[Serializable]
public class TextItem
{
    public string text;
}