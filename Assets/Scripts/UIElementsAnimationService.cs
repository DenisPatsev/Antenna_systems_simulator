using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIElementsAnimationService
{
    public static IEnumerator FadeIn(VisualElement element, float speed)
    {
        VisualElement visualElement = element;

        while (visualElement.resolvedStyle.opacity > 0)
        {
            visualElement.style.opacity = visualElement.resolvedStyle.opacity - speed * Time.deltaTime;

            yield return null;
        }
    }

    public static IEnumerator FadeOut(VisualElement element, float speed)
    {
        VisualElement visualElement = element;

        while (visualElement.resolvedStyle.opacity < 1)
        {
            visualElement.style.opacity = visualElement.resolvedStyle.opacity + speed * Time.deltaTime;

            yield return null;
        }
    }

    public static IEnumerator ScaleUp(VisualElement element, float maxScale, float speed)
    {
        VisualElement visualElement = element;
        float startScale = visualElement.resolvedStyle.scale.value.x;
        float targetScale = visualElement.resolvedStyle.scale.value.x * maxScale;

        while (visualElement.resolvedStyle.scale.value.x < targetScale)
        {
            startScale += Time.deltaTime * speed;
            visualElement.style.scale = new Scale(new Vector2(startScale, startScale));

            yield return null;
        }
    }

    public static IEnumerator ScaleDown(VisualElement element, float maxScale, float speed)
    {
        VisualElement visualElement = element;
        float startScale = visualElement.resolvedStyle.scale.value.x;
        float targetScale = visualElement.resolvedStyle.scale.value.x / maxScale;

        while (visualElement.resolvedStyle.scale.value.x < targetScale)
        {
            startScale -= Time.deltaTime * speed;
            visualElement.style.scale = new Scale(new Vector2(startScale, startScale));

            yield return null;
        }
    }

    public static IEnumerator PingPongScale(VisualElement element, float maxScale, float speed, int cycles)
    {
        VisualElement visualElement = element;
        float startScale = visualElement.resolvedStyle.scale.value.x;
        float currentScale = 0;
        float targetScale = 0;

        for (int i = 0; i < cycles; i++)
        {
            currentScale = startScale;
            targetScale = startScale * maxScale;
            
            while (visualElement.resolvedStyle.scale.value.x < targetScale)
            {
                currentScale += Time.deltaTime * speed;
                visualElement.style.scale = new Scale(new Vector2(currentScale, currentScale));

                yield return null;
            }

            targetScale = startScale;

            while (visualElement.resolvedStyle.scale.value.x > targetScale)
            {
                currentScale -= Time.deltaTime * speed;
                visualElement.style.scale = new Scale(new Vector2(currentScale, currentScale));

                yield return null;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    public static IEnumerator PingPongFade(VisualElement element, float speed, int cycles)
    {
        for (int i = 0; i < cycles; i++)
        {
            VisualElement visualElement = element;

            while (visualElement.resolvedStyle.opacity > 0)
            {
                visualElement.style.opacity = visualElement.resolvedStyle.opacity - speed * Time.deltaTime;

                yield return null;
            }

            while (visualElement.resolvedStyle.opacity < 1)
            {
                visualElement.style.opacity = visualElement.resolvedStyle.opacity + speed * Time.deltaTime;

                yield return null;
            }

            yield return null;
        }
    }
    
    public static IEnumerator ShuffleTextCoroutine(Label label,string text, float shuffleSpeed)
    {
        float timer = 0f;
        char[] tempArray = new char[text.Length];
        label.text = "";
        char[] symbols = Constants.ShufflerSymbols;
        WaitForSeconds wait = new WaitForSeconds(1/shuffleSpeed);

        for (int i = 0; i < text.Length; i++)
        {
            tempArray[i] = symbols[Random.Range(0, symbols.Length)];

            for (int j = 0; j < symbols.Length; j++)
            {
                tempArray[i] = symbols[Random.Range(0, symbols.Length)];
                label.text = tempArray.ArrayToString();
                yield return wait;
            }

            tempArray[i] = text[i];
            label.text = tempArray.ArrayToString();
        }
    }
}