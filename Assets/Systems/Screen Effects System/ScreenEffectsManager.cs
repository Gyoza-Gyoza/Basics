using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffectsManager : GameObjectPool
{
    public static ScreenEffectsManager Instance;

    [SerializeField]
    private Image screenCover;
    [SerializeField]
    private TextMeshProUGUI notification;
    [SerializeField]
    private Transform quickNotifications;

    //Title queue system
    private Queue<TaskSequence> titleTextQueue = new Queue<TaskSequence>(); //Holds the queue of title text sequences
    private bool isTitleActive = false;

    private TimerManager timerManager;
    private void Awake()
    {
        if (Instance == null) Instance = this; // Ensures only one instance of AudioSystem exists
        else Destroy(gameObject); // Destroys the object if an instance already exists
    }
    private void Start()
    {
        timerManager = TimerManager.Instance;
        screenCover.gameObject.SetActive(false);
        notification.gameObject.SetActive(false);
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
        
        timerManager.CreateTaskSequence(FadeSequence(screenCover, fadeInDuration, fadeOutDuration, holdDuration));
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
        TaskSequence sequence = new TaskSequence(InstantTask.Get(() => notification.text = notificationText));
        TaskSequence sequence = TaskSequence.Create(InstantTask.Create(() => notification.text = notificationText));
        sequence.AddTask(FadeSequence(notification, fadeInDuration, fadeOutDuration, holdDuration));
        titleTextQueue.Enqueue(sequence); //Adds the sequence to the queue

        TryRunNextNotification();
    }
    private void TryRunNextNotification()
    {
        if (isTitleActive || titleTextQueue.Count == 0) return; //If there is no notification to run, return

        TaskSequence nextSequence = TaskSequence.Create(titleTextQueue.Dequeue()); //Get the next sequence to run

        isTitleActive = true;

        //Adds recursion so the queue can automatically call the next task sequence
        nextSequence.AddTask(InstantTask.Create(() =>
        {
            isTitleActive = false;
            TryRunNextNotification();
        }));

        timerManager.CreateTaskSequence(nextSequence); //Adds it to the task list
    }

    /// <summary>
    /// Clears the title notification queue
    /// </summary>
    public void ClearTitleNotification()
    {
        titleTextQueue.Clear(); //Clears the queue of title text sequences
        notification.gameObject.SetActive(false); //Disables the notification text
    }
    public void CreateQuickTextNotification(Sprite icon, string notificationText, float fadeInDuration = 0.5f, float fadeOutDuration = 0.5f, float holdDuration = 1f)
    {
        GameObject textNotification = GetObject(); 

        textNotification.GetComponentInChildren<Image>().sprite = icon; 
        textNotification.GetComponentInChildren<TextMeshProUGUI>().text = notificationText;
        textNotification.transform.SetParent(quickNotifications, false);
        textNotification.transform.SetAsLastSibling();

        CanvasGroup canvasGroup = textNotification.GetComponent<CanvasGroup>();

        timerManager.CreateTaskSequence(FadeSequence(textNotification.GetComponent<CanvasGroup>(), fadeInDuration, fadeOutDuration, holdDuration));
    }

    private Task[] FadeSequence(Graphic target, float fadeInDuration, float fadeOutDuration, float holdDuration)
    {
        return new Task[]
        {
            InstantTask.Create(() => target.gameObject.SetActive(true)),
            CreateFadeRoutine(target, fadeInDuration, fadeOutDuration, true),
            Wait.Create(holdDuration),
            CreateFadeRoutine(target, fadeInDuration, fadeOutDuration, false),
            InstantTask.Create(() => target.gameObject.SetActive(false))
        };
    }
    private Task[] FadeSequence(CanvasGroup target, float fadeInDuration, float fadeOutDuration, float holdDuration)
    {
        return new Task[]
        {
            InstantTask.Create(() => target.gameObject.SetActive(true)),
            CreateFadeRoutine(target, fadeInDuration, fadeOutDuration, true),
            Wait.Create(holdDuration),
            CreateFadeRoutine(target, fadeInDuration, fadeOutDuration, false),
            InstantTask.Create(() => ReturnObject(target.gameObject))
        };
    }
    /// <summary>
    /// Used to fade a UI element in or out
    /// </summary>
    /// <param name="target">Target to fade</param>
    /// <param name="fadeInDuration">Duration of the fade in</param>
    /// <param name="fadeOutDuration">Duration of the fade out</param>
    /// <param name="fadeIn">True to fade in, False to fade out</param>
    /// <returns></returns>
    private void Fade(Graphic target, float fadeInDuration, float fadeOutDuration, bool fadeIn)
    {
        timerManager.Tasks.Add(CreateFadeRoutine(target, fadeInDuration, fadeOutDuration, fadeIn));
    }
    private RoutineTask CreateFadeRoutine(Graphic target, float fadeInDuration, float fadeOutDuration, bool fadeIn)
    {
        float timer = 0f;

        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;
        float duration = fadeIn ? fadeInDuration : fadeOutDuration;

        if (duration <= 0f) //Prevent lerp issues when dividing by 0
        {
            target.color =
                    new Color(target.color.r,
                    target.color.g,
                    target.color.b,
                    endAlpha);
            return RoutineTask.Create(0f, null);
        }

        return RoutineTask.Create(duration, () =>
        {
            if (timer <= duration)
            {
                timer += Time.deltaTime;

                target.color =
                    new Color(target.color.r,
                    target.color.g,
                    target.color.b,
                    Mathf.Lerp(startAlpha, endAlpha, timer / duration));
            }
        });
    }

    /// <summary>
    /// Used to fade a Canvas Group in or out
    /// </summary>
    /// <param name="target">Target to fade</param>
    /// <param name="fadeInDuration">Duration of the fade in</param>
    /// <param name="fadeOutDuration">Duration of the fade out</param>
    /// <param name="fadeIn">True to fade in, False to fade out</param>
    /// <returns></returns>
    private void Fade(CanvasGroup target, float fadeInDuration, float fadeOutDuration, bool fadeIn)
    {
        timerManager.Tasks.Add(CreateFadeRoutine(target, fadeInDuration, fadeOutDuration, fadeIn));
    }
    private RoutineTask CreateFadeRoutine(CanvasGroup target, float fadeInDuration, float fadeOutDuration, bool fadeIn)
    {
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;
        float duration = fadeIn ? fadeInDuration : fadeOutDuration;

        if (duration <= 0f) //Prevent lerp issues when dividing by 0
        {
            target.alpha = endAlpha;
            return RoutineTask.Create(0f, null);
        }

        float timer = 0f;

        return RoutineTask.Create(duration, () =>
        {
            if (timer <= duration)
            {
                timer += Time.deltaTime;

                target.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            }
        });
    }
}
