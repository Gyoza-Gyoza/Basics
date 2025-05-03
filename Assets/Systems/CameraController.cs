using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    // Variables for controlling the camera 
    [SerializeField]
    private float xSensitivity = 5f, ySensitivity = 5f;
    // Movement variables
    private float xCursor, yCursor, upDownRotation,
        yLookMin = -80f, yLookMax = 80f;

    [SerializeField]
    private Vector3 cameraShakeMagnitude;

    private GameObject target;
    private TimerManager timerManager;

    private void Start()
    {
        timerManager = TimerManager.Instance;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) CameraShake(1f, cameraShakeMagnitude);
        MouseInput();
    }
    private void MouseInput() //Gets the mouse position 
    {
        xCursor = Input.GetAxis("Mouse X") * xSensitivity; // Gets mouse X input
        yCursor = Input.GetAxis("Mouse Y") * ySensitivity; // Gets mouse Y input

        // Performs vertical rotation
        upDownRotation += -yCursor;
        upDownRotation = Mathf.Clamp(upDownRotation, yLookMin, yLookMax);
        transform.localRotation = Quaternion.Euler(upDownRotation, 0f, 0f);

        player.transform.Rotate(Vector3.up, xCursor); // Performs horizontal rotation
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
        return transform.position;
    }
}
