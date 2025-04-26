using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity
{
    [SerializeField]
    private float jumpHeight = 5f;
    private InputManager inputManager;
    private Rigidbody rb;
    private PlayerState playerState = PlayerState.Idle;
    public PlayerState PlayerState
    {
        get { return playerState; }
        set { playerState = value; }
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    protected override void Start()
    {
        base.Start();
        inputManager = InputManager.Instance;
        Debug.Log("Player start called");
    }
    private void Update()
    {
        // Handle player input and update player state
        HandleInput();
    }
    private void HandleInput()
    {
        if(Input.GetKey(inputManager.GetKey(KeyInput.Forward))) 
            rb.MovePosition(rb.position + new Vector3(0, 0, 1) * MovementSpeed * Time.deltaTime);
        else if(Input.GetKey(inputManager.GetKey(KeyInput.Backward))) 
            rb.MovePosition(rb.position + new Vector3(0, 0, -1) * MovementSpeed * Time.deltaTime);
        else if(Input.GetKey(inputManager.GetKey(KeyInput.Right))) 
            rb.MovePosition(rb.position + new Vector3(1, 0, 0) * MovementSpeed * Time.deltaTime);
        else if(Input.GetKey(inputManager.GetKey(KeyInput.Left))) 
            rb.MovePosition(rb.position + new Vector3(-1, 0, 0) * MovementSpeed * Time.deltaTime);

        if (Input.GetKeyDown(inputManager.GetKey(KeyInput.Jump))) Jump(jumpHeight);
    }
    public void Jump(float amount)
    {
        PlayerState = PlayerState.Jumping;
        rb.AddForce(Vector3.up * amount, ForceMode.Impulse);
    }

    protected override void OnHeal()
    {

    }

    protected override void OnDamage()
    {

    }

    public override void OnDeath()
    {

    }
}
public enum PlayerState
{
    Idle,
    Walking,
    Running,
    Jumping,
    Falling
}