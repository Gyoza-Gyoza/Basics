using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffectsManager : MonoBehaviour
{
    public static ScreenEffectsManager Instance;

    [SerializeField]
    private Image screenCover;
    [SerializeField]
    private TextMeshProUGUI message;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P)) ScreenFade();
    }
    public void ScreenFade(float fadeDuration = 0.5f, float holdDuration = 1f, Color? color = null)
    {
        //Assigns chosen values to the screen 
        screenCover.color = color ?? Color.black;
        
        StartCoroutine(ScreenFadeCoroutine(fadeDuration, holdDuration));
    }
    private IEnumerator ScreenFadeCoroutine(float fadeDuration, float holdDuration)
    {
        //Fade in
        screenCover.gameObject.SetActive(true);
        StartCoroutine(Fade(screenCover, fadeDuration, true));

        //Hold
        yield return new WaitForSeconds(holdDuration);

        //Fade out
        yield return StartCoroutine(Fade(screenCover, fadeDuration, false));
        screenCover.gameObject.SetActive(false);
    }

    /// <summary>
    /// Used to fade a UI element in or out
    /// </summary>
    /// <param name="target">Target to fade</param>
    /// <param name="fadeDuration">Duration of the fade</param>
    /// <param name="fadeIn">True to fade in, False to fade out</param>
    /// <returns></returns>
    private IEnumerator Fade(Graphic target, float fadeDuration, bool fadeIn)
    {
        float timer = 0f;

        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        while (timer <= fadeDuration)
        {
            timer += Time.deltaTime;

            target.color =
                new Color(target.color.r,
                target.color.g,
                target.color.b,
                Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration));

            yield return null;
        }
    }
}
