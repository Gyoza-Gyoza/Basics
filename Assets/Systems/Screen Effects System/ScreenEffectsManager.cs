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
    private TextMeshProUGUI notification;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void Start()
    {
        screenCover.gameObject.SetActive(false);
        notification.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P)) ScreenFade();
        if (Input.GetKeyUp(KeyCode.O)) CreateTextNotification("Test");
    }

    /// <summary>
    /// Performs a full screen fade in and out
    /// </summary>
    /// <param name="fadeDuration">Duration taken to fade in/out</param>
    /// <param name="holdDuration">Duration that the fade will hold for</param>
    /// <param name="color">Color of the screen fade, leave as null for black</param>
    public void ScreenFade(float fadeDuration = 0.5f, float holdDuration = 1f, Color? color = null)
    {
        //Assigns chosen values to the screen 
        screenCover.color = color ?? Color.black;
        
        StartCoroutine(FadeCoroutine(screenCover, fadeDuration, holdDuration));
    }

    /// <summary>
    /// Creates a text notification 
    /// </summary>
    /// <param name="notificationText">Text to display</param>
    /// <param name="fadeDuration">Duration taken to fade in/out</param>
    /// <param name="holdDuration">Duration that the fade will hold for</param>
    public void CreateTextNotification(string notificationText, float fadeDuration = 0.5f, float holdDuration = 1f)
    {
        //Assigns text to the notification
        notification.text = notificationText;

        StartCoroutine(FadeCoroutine(notification, fadeDuration, holdDuration));
    }

    private IEnumerator FadeCoroutine(Graphic target, float fadeDuration, float holdDuration)
    {
        //Fade in
        target.gameObject.SetActive(true);
        StartCoroutine(Fade(target, fadeDuration, true));

        //Hold
        yield return new WaitForSeconds(holdDuration);

        //Fade out
        yield return StartCoroutine(Fade(target, fadeDuration, false));
        target.gameObject.SetActive(false);
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
