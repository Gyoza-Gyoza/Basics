using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;
    private List<TimedAction> timedActions = new List<TimedAction>();
    private List<TimedRoutine> timedRoutines = new List<TimedRoutine>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Update()
    {
        foreach (var action in timedActions)
        {
            if (action.isActive)
            {
                action.elapsedTime += Time.deltaTime;
                if (action.elapsedTime >= action.duration)
                {
                    action.onComplete?.Invoke();
                    action.isActive = false;
                }
            }
        }
        foreach(var routine in timedRoutines)
        {
            if (routine.isActive)
            {
                routine.elapsedTime += Time.deltaTime;
                if (routine.elapsedTime <= routine.duration) routine.routine?.Invoke();
                else routine.isActive = false;
            }
        }
    }
    #region Object Pooling
    /// <summary>
    /// Get a timed action from the pool
    /// </summary>
    /// <param name="duration">Duration before action will be performed</param>
    /// <param name="onComplete">Action that will be performed after duration has passed</param>
    /// <returns></returns>
    public void CreateTimedAction(float duration, Action onComplete)
    {
        foreach(var timedAction in timedActions)
        {
            //If there is an available object, reuse it
            if (!timedAction.isActive)
            {
                timedAction.isActive = true;
                timedAction.duration = duration;
                timedAction.elapsedTime = 0f;
                timedAction.onComplete = onComplete;
                return;
            }
        }
        //If there are no available objects, create a new one
        timedActions.Add(new TimedAction(duration, onComplete));
    }

    /// <summary>
    /// Get a timed routine from the pool
    /// </summary>
    /// <param name="duration">Duration of the routine being performed</param>
    /// <param name="routine">Action that will be performed over the duration of the routine</param>
    /// <returns></returns>
    public void CreateTimedRoutine(float duration, Action routine)
    {
        foreach (var timedRoutine in timedRoutines)
        {
            //If there is an available object, reuse it
            if (!timedRoutine.isActive)
            {
                timedRoutine.isActive = true;
                timedRoutine.duration = duration;
                timedRoutine.elapsedTime = 0f;
                timedRoutine.routine = routine;
                return;
            }
        }
        //If there are no available objects, create a new one
        timedRoutines.Add(new TimedRoutine(duration, routine));
    }
    #endregion
}

public class TimedAction
{
    public float duration;
    public float elapsedTime;
    public bool isActive;
    public Action onComplete;
    public TimedAction(float duration, Action onComplete)
    {
        this.duration = duration;
        this.onComplete = onComplete;
        isActive = true;
        elapsedTime = 0f;
    }
}
public class TimedRoutine
{
    public float duration;
    public float elapsedTime;
    public bool isActive;
    public Action routine;
    public TimedRoutine(float duration, Action routine)
    {
        this.duration = duration;
        this.routine = routine;
        isActive = true;
        elapsedTime = 0f;
    }
}