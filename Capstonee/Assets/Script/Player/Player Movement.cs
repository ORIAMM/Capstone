using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speed")]
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;
    private float speed;

    [Header("Stamina")]
    public float playerStamina = 100.0f;
    [SerializeField] private float maxStamina = 100.0f;
    [HideInInspector] public bool hasRegen = true;
    [Range(0, 50)][SerializeField] private float staminaDrain;
    [Range(0, 50)][SerializeField] private float staminaRegen;


    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Keybinds")]
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("References")]
    public Transform orientation;

    float HorInput;
    float VerInput;

    Vector3 moveDir;

    Rigidbody rb;

    public static MovementState state;
    public enum MovementState
    {
        walking,
        sprinting
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Ground Check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        MyInput();
        SpeedControl();
        StateHandler();

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        HorInput = Input.GetAxisRaw("Horizontal");
        VerInput = Input.GetAxisRaw("Vertical");
    }

    private void StateHandler()
    {
        if (grounded && Input.GetKey(sprintKey) && hasRegen)
        {
            Debug.Log("Sprinting");
            state = MovementState.sprinting;
            speed = sprintSpeed;
            playerStamina -= staminaDrain * Time.deltaTime;
            //UpdateStamina(1);
            if (playerStamina <= 0)
            {
                hasRegen = false;
            }

        }

        else if (grounded)
        {
            state = MovementState.walking;
            speed = walkSpeed;
            if (playerStamina <= maxStamina - 0.01)
            {
                //UpdateStamina(1);
                playerStamina += staminaRegen * Time.deltaTime;
                if (playerStamina >= maxStamina)
                {
                    //UpdateStamina(0);
                    hasRegen = true;
                }
            }
        }
    }

/*    void UpdateStamina(int value)
    {
        staminaProg.fillAmount = playerStamina / maxStamina;

        if (value == 0)
        {
            sliderCanvasGroup.DOFade(0, 0.5f);
            *//*sliderCanvasGroup.alpha = 0;*//*
        }
        else
        {
            sliderCanvasGroup.DOFade(1, 0.5f);
            *//*sliderCanvasGroup.alpha = 1;*//*
        }
    }*/


    private void MovePlayer()
    {
        moveDir = orientation.forward * VerInput + orientation.right * HorInput;

        rb.AddForce(moveDir.normalized * speed * 5f, ForceMode.Force);

    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.y);

        // Limitation
        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
}
