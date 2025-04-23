using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;
    private List<TimedTask> timedTasks = new List<TimedTask>();
    public List<TimedTask> TimedTasks
    { get { return timedTasks; } }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Update()
    {
        for (int i = timedTasks.Count - 1; i >= 0; i--)
        {
            if (timedTasks[i].IsActive) timedTasks[i].Tick();
            else timedTasks.RemoveAt(i);
        }
    }
    #region Object Pooling
    /// <summary>
    /// Initializes a new timed action
    /// </summary>
    /// <param name="duration">Duration before action will be performed</param>
    /// <param name="onComplete">Action that will be performed after duration has passed</param>
    /// <returns></returns>
    public void CreateTimedAction(float duration, Action onComplete)
    {
        timedTasks.Add(TimedAction.Get(duration, onComplete));
    }

    /// <summary>
    /// Get a timed routine from the pool
    /// </summary>
    /// <param name="duration">Duration of the routine being performed</param>
    /// <param name="routine">Action that will be performed over the duration of the routine</param>
    /// <returns></returns>
    public void CreateTimedRoutine(float duration, Action routine)
    {
        timedTasks.Add(TimedRoutine.Get(duration, routine));
    }
    #endregion
}

public abstract class TimedTask
{
    public float Duration;
    public float ElapsedTime;
    public bool IsActive;

    public TimedTask(float duration)
    {
        Duration = duration;
        ElapsedTime = 0f;
        IsActive = true;
    }
    public abstract void Tick();
}
public class TimedAction : TimedTask
{
    private static Stack<TimedAction> timedActions = new Stack<TimedAction>();

    public Action OnComplete;
    private TimedAction(float duration, Action onComplete) : base(duration)
    {
        OnComplete = onComplete;
    }

    /// <summary>
    /// Returns a TimedAction
    /// </summary>
    /// <param name="duration">Duration before action will be performed</param>
    /// <param name="onComplete">Action that will be performed after duration has passed</param>
    /// <returns></returns>
    public static TimedAction Get(float duration, Action onComplete)
    {
        if (timedActions.Count > 0) //Checks if there are objects in the pool 
        {
            TimedAction result = timedActions.Pop();

            result.Duration = duration;
            result.ElapsedTime = 0f;
            result.IsActive = true;
            result.OnComplete = onComplete;

            return result;
        }
        else return new TimedAction(duration, onComplete);
    }
    public void Return()
    {
        timedActions.Push(this);
    }
    public override void Tick()
    {
        ElapsedTime += Time.deltaTime;
        if (ElapsedTime >= Duration)
        {
            OnComplete?.Invoke();
            IsActive = false;
            Return();
        }
    }
}
public class TimedRoutine : TimedTask
{
    private static Stack<TimedRoutine> timedRoutines = new Stack<TimedRoutine>();

    public Action Routine;
    private TimedRoutine(float duration, Action routine) : base(duration) 
    {
        Routine = routine;
    }

    /// <summary>
    /// Returns a TimedRoutine 
    /// </summary>
    /// <param name="duration">Duration of the routine being performed</param>
    /// <param name="onComplete">Action that will be performed over the duration of the routine</param>
    /// <returns></returns>
    public static TimedRoutine Get(float duration, Action routine)
    {
        if (timedRoutines.Count > 0) //Checks if there are objects in the pool 
        {
            TimedRoutine result = timedRoutines.Pop();

            result.Duration = duration;
            result.ElapsedTime = 0f;
            result.IsActive = true;
            result.Routine = routine;

            return result;
        }
        else return new TimedRoutine(duration, routine);
    }
    public void Return()
    {
        timedRoutines.Push(this);
    }
    public override void Tick()
    {
        ElapsedTime += Time.deltaTime;
        if (ElapsedTime <= Duration) Routine?.Invoke();
        else
        {
            IsActive = false;
            Return();
        }
    }
}