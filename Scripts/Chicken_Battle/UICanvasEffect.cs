using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasEffect : MonoBehaviour
{
    private RectTransform rectTransform;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private Vector2 GetRandomPosition()
    {
        // Get the size of the canvas
        float canvasWidth = rectTransform.rect.width;
        float canvasHeight = rectTransform.rect.height;

        // Generate random x and y positions within the canvas bounds
        float randomX = Random.Range(-canvasWidth / 2, canvasWidth / 2);
        float randomY = Random.Range(-canvasHeight / 2, canvasHeight / 2);

        // Return the random position as a Vector2
        return new Vector2(randomX, randomY);
    }

    public void Display(string textEffect, Color color) {
        if (rectTransform == null) {
            return;
        }
        
        GameObject effect = new GameObject("effectText", typeof(RectTransform));
        effect.transform.SetParent(rectTransform);

        RectTransform rect = effect.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = new Vector3(2, 2, 2); // Reset scale
        rect.anchorMin = new Vector2(0.5f, 0.5f); // Centered anchor
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(1, 0.15f);

        effect.transform.position = GetRandomPosition();

        TMP_Text text = effect.AddComponent<TextMeshPro>();
        text.text = textEffect;
        text.color = color;
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 2f;

        StartCoroutine(MoveAndFade(effect));
    }

    private IEnumerator MoveAndFade(GameObject effect)
    {
        float moveDistance = 1f;
        float duration = 2f;
        TMP_Text text = effect.GetComponent<TMP_Text>();
        Color startColor = text.color;
        Vector3 startPosition = effect.transform.position;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Move the text upwards
            effect.transform.localPosition = startPosition + Vector3.up * moveDistance * t;

            // Fade out the text
            // Color newColor = startColor;
            // newColor.a = Mathf.Lerp(1f, 0.5f, t);
            // text.color = newColor;

            yield return null;
        }

        Destroy(effect);
    }
}
