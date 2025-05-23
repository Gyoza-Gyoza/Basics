using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity
{
    [SerializeField]
    private float jumpHeight = 5f;
    [SerializeField]
    private float sprintMultiplier = 2f;
    private bool isSprinting = false;

    [Header("Ground check variables")]
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private LayerMask groundLayer;
    private bool isGrounded = false;

    private InputManager inputManager;
    private Rigidbody rb;
    private PlayerState playerState = PlayerState.Idle;
    public PlayerState PlayerState
    {
        get { return playerState; }
        set { playerState = value; }
    }
    private Vector3 movement;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    protected override void Start()
    {
        base.Start();
        inputManager = InputManager.Instance;
    }
    private void Update()
    {
        GroundCheck();
        Movement();
    }
    private void Movement()
    {
        // Handles the basic player movement 

        movement = Vector3.zero; // Resets the movement value

        // Gets each axis input
        if (Input.GetKey(inputManager.GetKey(KeyInput.Forward))) movement.z += 1f;
        else if (Input.GetKey(inputManager.GetKey(KeyInput.Backward))) movement.z -= 1f;

        if (Input.GetKey(inputManager.GetKey(KeyInput.Right))) movement.x += 1f;
        else if (Input.GetKey(inputManager.GetKey(KeyInput.Left))) movement.x -= 1f;

        if (Input.GetKey(inputManager.GetKey(KeyInput.Sprint)))
        {
            isSprinting = true;
            PlayerState = PlayerState.Sprinting;
        }
        else
        {
            isSprinting = false;
            PlayerState = PlayerState.Walking;
        }

        movement = (transform.right * movement.x + transform.forward * movement.z).normalized; // Calculates the movement of each axis and normalizes it 

        float movementMultiplier = isSprinting ? MovementSpeed * sprintMultiplier: MovementSpeed; // Calculates the movement speed 

        rb.MovePosition(rb.position + movement * movementMultiplier * Time.deltaTime); // Moves the rigidbody of the player based on the calculated movement 

        if (Input.GetKeyDown(inputManager.GetKey(KeyInput.Jump)) && isGrounded) Jump(jumpHeight);
    }
    private void GroundCheck()
    {
        isGrounded = Physics.CheckBox(groundCheck.position, transform.localScale / 2, Quaternion.identity, groundLayer);
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
    Sprinting,
    Jumping,
    Falling
}