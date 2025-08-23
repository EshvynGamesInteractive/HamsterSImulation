using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Typewriter : MonoBehaviour
{
    public static Typewriter instance;

    [SerializeField] private Text textBox;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] CanvasGroup bg;

    private Coroutine typingCoroutine;
    public bool autoHideAfterCompletion = true;

    private void Awake()
    {
        instance = this;
    }

    public void StartTyping(string message, float delay)
    {
        if (typingCoroutine != null)
            // return;
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message, delay));
    }

    private IEnumerator TypeText(string message, float delay)
    {
        yield return new WaitForSeconds(delay);
        bg.DOFade(1, 0.1f);
        textBox.text = "";
        foreach (char letter in message)
        {
            textBox.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(3);
        if (autoHideAfterCompletion)
            bg.DOFade(0, 0.1f);
        typingCoroutine = null;
    }

    public void HideTypeWriter()
    {
        autoHideAfterCompletion = true;
        bg.DOFade(0, 0.1f);
    }
}