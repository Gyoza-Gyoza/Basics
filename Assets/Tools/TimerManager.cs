using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;
    private List<Task> tasks = new List<Task>();
    public List<Task> Tasks
    { get { return tasks; } }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Update()
    {
        //Reverse for loop to avoid index issues when removing items from the list
        for (int i = tasks.Count - 1; i >= 0; i--)
        {
            if (tasks[i].IsActive) tasks[i].Tick(); //Calls the tick function of the task
            else tasks.RemoveAt(i); //Removes the task from the list if it is inactive
        }
    }
    /// <summary>
    /// Initializes a new timed action
    /// </summary>
    /// <param name="duration">Duration before action will be performed</param>
    /// <param name="onComplete">Action that will be performed after duration has passed</param>
    /// <returns></returns>
    public void CreateTimedAction(float duration, Action onComplete)
    {
        tasks.Add(TimedTask.Get(duration, onComplete));
    }

    /// <summary>
    /// Initializes a new timed routine
    /// </summary>
    /// <param name="duration">Duration of the routine being performed</param>
    /// <param name="routine">Action that will be performed over the duration of the routine</param>
    /// <returns></returns>
    public void CreateTimedRoutine(float duration, Action routine)
    {
        tasks.Add(RoutineTask.Get(duration, routine));
    }

    /// <summary>
    /// Initializes a new task sequence
    /// </summary>
    /// <param name="sequence">List of tasks to be performed</param>
    /// <returns></returns>
    public void CreateTaskSequence(Task[] sequence)
    {
        tasks.Add(new TaskSequence(sequence));
    }
}

public abstract class Task
{
    public bool IsActive; //Whether the task is active or not
    public Task()
    {
        IsActive = true;
    }
    //Creates a common function that will be called each frame in the update loop
    public abstract void Tick();
}

public class TimedTask : Task
{
    private static Stack<TimedTask> timedActions = new Stack<TimedTask>();

    public float Duration; //Duration of the task
    public float ElapsedTime; //Current time elapsed
    public Action OnComplete;

    private TimedTask(float duration, Action onComplete)
    {
        Duration = duration;
        ElapsedTime = 0f;
        OnComplete = onComplete;
    }

    /// <summary>
    /// Returns a TimedAction
    /// </summary>
    /// <param name="duration">Duration before action will be performed</param>
    /// <param name="onComplete">Action that will be performed after duration has passed</param>
    /// <returns></returns>
    public static TimedTask Get(float duration, Action onComplete)
    {
        if (timedActions.Count > 0) //Checks if there are objects in the pool 
        {
            TimedTask result = timedActions.Pop();

            result.Duration = duration;
            result.ElapsedTime = 0f;
            result.IsActive = true;
            result.OnComplete = onComplete;

            return result;
        }
        else return new TimedTask(duration, onComplete);
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

/// <summary>
/// Performs actions over a certain amount of time. Use Get function to get a new instance of the class.
/// </summary>
public class RoutineTask : Task
{
    private static Stack<RoutineTask> timedRoutines = new Stack<RoutineTask>();

    public float Duration; //Duration of the task
    public float ElapsedTime; //Current time elapsed
    public Action Routine;
    private RoutineTask(float duration, Action routine)
    {
        Duration = duration;
        ElapsedTime = 0f;
        Routine = routine;
    }

    /// <summary>
    /// Returns a TimedRoutine 
    /// </summary>
    /// <param name="duration">Duration of the routine being performed</param>
    /// <param name="onComplete">Action that will be performed over the duration of the routine</param>
    /// <returns></returns>
    public static RoutineTask Get(float duration, Action routine)
    {
        if (timedRoutines.Count > 0) //Checks if there are objects in the pool 
        {
            RoutineTask result = timedRoutines.Pop();

            result.Duration = duration;
            result.ElapsedTime = 0f;
            result.IsActive = true;
            result.Routine = routine;

            return result;
        }
        else return new RoutineTask(duration, routine);
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

/// <summary>
/// Performs a sequence of tasks
/// </summary>
public class TaskSequence : Task
{
    private Queue<Task> taskQueue = new Queue<Task>();

    public bool IsComplete
    { get { return taskQueue.Count == 0; } }

    public TaskSequence(Task[] sequence) : base()
    {
        foreach (Task task in sequence)
        {
            taskQueue.Enqueue(task);
        }
        IsActive = true;
    }

    public void AddTask(TimedTask task) => taskQueue.Enqueue(task);
    public override void Tick()
    {
        if(taskQueue.Count > 0)
        {
            Task currentTask = taskQueue.Peek();

            if(currentTask == null)
            {
                taskQueue.Dequeue();
                return;
            }

            currentTask.Tick();

            if (currentTask.IsActive == false)
            {
                taskQueue.Dequeue();
                if (taskQueue.Count == 0) IsActive = false;
            }
        }
    }
}

/// <summary>
/// Waits for a certain amount of time. Use Get function to get a new instance of the class.
/// </summary>
public class Wait : Task
{
    private static Stack<Wait> waits = new Stack<Wait>();

    public float Duration; //Duration of the task
    public float ElapsedTime; //Current time elapsed
    private Wait(float duration)
    {
        Duration = duration;
        ElapsedTime = 0f;
    }
    public static Wait Get(float duration)
    {
        if (waits.Count > 0) //Checks if there are objects in the pool 
        {
            Wait result = waits.Pop();
            result.Duration = duration;
            result.ElapsedTime = 0f;
            result.IsActive = true;
            return result;
        }
        else return new Wait(duration);
    }
    public override void Tick()
    {
        ElapsedTime += Time.deltaTime;
        if (ElapsedTime >= Duration)
        {
            IsActive = false;
        }
    }
    public void Return()
    {
        waits.Push(this);
    }
}

/// <summary>
/// Performs an action instantly. Use Get function to get a new instance of the class.
/// </summary>
public class InstantTask : Task
{
    private static Stack<InstantTask> instantTasks = new Stack<InstantTask>();
    public Action OnComplete;
    public InstantTask(Action task)
    {
        OnComplete = task;
    }
    public override void Tick()
    {
        OnComplete?.Invoke();
        IsActive = false;
    }
    public static InstantTask Get(Action task)
    {
        if (instantTasks.Count > 0) //Checks if there are objects in the pool 
        {
            InstantTask result = instantTasks.Pop();
            result.IsActive = true;
            result.OnComplete = task;
            return result;
        }
        else return new InstantTask(task);
    }
    public void Return()
    {
        IsActive = false;
        instantTasks.Push(this);
    }
}