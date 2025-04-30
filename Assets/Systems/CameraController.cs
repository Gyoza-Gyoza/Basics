using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset;

    private GameObject target;
    private TimerManager timerManager;
    public static CameraController Instance;

    public void Awake()
    {
        if (Instance == null) Instance = this; // Ensures only one instance of CameraEffects exists
        else Destroy(gameObject); // Destroys the object if an instance already exists
    }
    private void Start()
    {
        timerManager = TimerManager.Instance;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) CameraShake(1f, offset);
    }
    public void CameraShake(float duration, Vector2 magnitude)
    {
        Vector3 originalPosition = GetCameraPosition();
        timerManager.CreateRoutineTask(duration, () =>
        {
            transform.position = new Vector3(originalPosition.x + Random.Range(-magnitude.x, magnitude.x), originalPosition.y + Random.Range(-magnitude.y, magnitude.y), originalPosition.z);
        });
    }
    public void SwitchTarget(float duration, Transform target)
    {
        float timer = 0f;
        timerManager.CreateRoutineTask(duration, () =>
        {
            transform.position = Vector3.Lerp(GetCameraPosition(), target.position, timer / duration);
            if (timer >= duration) this.target = target.gameObject;
        });
    }
    public Vector3 GetCameraPosition()
    {
        return transform.position + offset;
    }
}
