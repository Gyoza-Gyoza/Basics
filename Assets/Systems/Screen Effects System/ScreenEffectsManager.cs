using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ScreenEffectsManager : GameObjectPool
{
    public static ScreenEffectsManager Instance;

    [SerializeField]
    private Image screenCover;
    [SerializeField]
    private TextMeshProUGUI notification;
    [SerializeField]
    private Transform quickNotifications;

    private TimerManager timerManager;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        timerManager = TimerManager.Instance;
    }
    private void Start()
    {
        screenCover.gameObject.SetActive(false);
        notification.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P)) ScreenFade();
        if (Input.GetKeyUp(KeyCode.O)) CreateTitleTextNotification("Test");
        if (Input.GetKeyUp(KeyCode.I)) CreateQuickTextNotification(null, "Test", 0, 1);
    }

    /// <summary>
    /// Performs a full screen fade in and out
    /// </summary>
    /// <param name="fadeInDuration">Duration taken to fade in</param>
    /// <param name="fadeOutDuration">Duration taken to fade out</param>
    /// <param name="holdDuration">Duration that the fade will hold for</param>
    /// <param name="color">Color of the screen fade, leave as null for black</param>
    public void ScreenFade(float fadeInDuration = 0.5f, float fadeOutDuration = 0.5f, float holdDuration = 1f, Color? color = null)
    {
        //Assigns chosen values to the screen 
        screenCover.color = color ?? Color.black;
        
        StartCoroutine(FadeCoroutine(screenCover, fadeInDuration, fadeOutDuration, holdDuration));
    }

    /// <summary>
    /// Creates a title text notification 
    /// </summary>
    /// <param name="notificationText">Text to display</param>
    /// <param name="fadeInDuration">Duration taken to fade in</param>
    /// <param name="fadeOutDuration">Duration taken to fade out</param>
    /// <param name="holdDuration">Duration that the fade will hold for</param>
    public void CreateTitleTextNotification(string notificationText, float fadeInDuration = 0.5f, float fadeOutDuration = 0.5f, float holdDuration = 1f)
    {
        //Assigns text to the notification
        notification.text = notificationText;

        StartCoroutine(FadeCoroutine(notification, fadeInDuration, fadeOutDuration, holdDuration));
    }
    public void CreateQuickTextNotification(Sprite icon, string notificationText, float fadeInDuration = 0.5f, float fadeOutDuration = 0.5f, float holdDuration = 1f)
    {
        GameObject textNotification = GetObject(); 

        textNotification.GetComponentInChildren<Image>().sprite = icon; 
        textNotification.GetComponentInChildren<TextMeshProUGUI>().text = notificationText;
        textNotification.transform.SetParent(quickNotifications, false);

        StartCoroutine(FadeCoroutine(textNotification.GetComponent<CanvasGroup>(), fadeInDuration, fadeOutDuration, holdDuration));
        timerManager.CreateTimedAction(fadeInDuration + fadeOutDuration + holdDuration, () => DestroyObject(textNotification));
    }

    private IEnumerator FadeCoroutine(Graphic target, float fadeInDuration, float fadeOutDuration, float holdDuration)
    {
        //Fade in
        target.gameObject.SetActive(true);
        StartCoroutine(Fade(target, fadeInDuration, fadeOutDuration, true));

        //Hold
        yield return new WaitForSeconds(holdDuration);

        //Fade out
        yield return StartCoroutine(Fade(target, fadeInDuration, fadeOutDuration, false));
        target.gameObject.SetActive(false);
    }
    private IEnumerator FadeCoroutine(CanvasGroup target, float fadeInDuration, float fadeOutDuration, float holdDuration)
    {
        //Fade in
        target.gameObject.SetActive(true);
        StartCoroutine(Fade(target, fadeInDuration, fadeOutDuration, true));

        //Hold
        yield return new WaitForSeconds(holdDuration);

        //Fade out
        yield return StartCoroutine(Fade(target, fadeInDuration, fadeOutDuration, false));
        target.gameObject.SetActive(false);
    }
    private IEnumerator QuickTextCoroutine(GameObject target, float fadeInDuration, float fadeOutDuration, float holdDuration)
    {
        yield return StartCoroutine(FadeCoroutine(target.GetComponent<CanvasGroup>(), fadeInDuration, fadeOutDuration, holdDuration));
        
        DestroyObject(target);
        //Change this coroutine into a timed action
    }
    /// <summary>
    /// Used to fade a UI element in or out
    /// </summary>
    /// <param name="target">Target to fade</param>
    /// <param name="fadeInDuration">Duration of the fade in</param>
    /// <param name="fadeOutDuration">Duration of the fade out</param>
    /// <param name="fadeIn">True to fade in, False to fade out</param>
    /// <returns></returns>
    private IEnumerator Fade(Graphic target, float fadeInDuration, float fadeOutDuration, bool fadeIn)
    {
        float timer = 0f;
        
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;
        float duration = fadeIn ? fadeInDuration : fadeOutDuration;

        while (timer <= duration)
        {
            timer += Time.deltaTime;

            target.color =
                new Color(target.color.r,
                target.color.g,
                target.color.b,
                Mathf.Lerp(startAlpha, endAlpha, timer / duration));

            yield return null;
        }
    }

    /// <summary>
    /// Used to fade a Canvas Group in or out
    /// </summary>
    /// <param name="target">Target to fade</param>
    /// <param name="fadeInDuration">Duration of the fade in</param>
    /// <param name="fadeOutDuration">Duration of the fade out</param>
    /// <param name="fadeIn">True to fade in, False to fade out</param>
    /// <returns></returns>
    private IEnumerator Fade(CanvasGroup target, float fadeInDuration, float fadeOutDuration, bool fadeIn)
    {
        float timer = 0f;
        
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;
        float duration = fadeIn ? fadeInDuration : fadeOutDuration;

        while (timer <= duration)
        {
            timer += Time.deltaTime;

            target.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);

            yield return null;
        }
    }
}
