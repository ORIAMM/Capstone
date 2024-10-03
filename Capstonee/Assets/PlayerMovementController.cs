using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    //State : 2D or 3D
    //if 2D, can only target 2D entities but can see the shadows in 3D env
    //if 3D, can only target 3D entities but can see the 2D ground
    [Header("Player Config")]
    [SerializeField] private float player_speed;
    [SerializeField] private float airmultiplier;
    [SerializeField] private float resettime;
    [SerializeField] private float jump_power;
    [SerializeField] private float gravity;
    [SerializeField] private float friction;
    [Space]
    [Header("Camera Config")]
    public Camera playercam;
    public float camera_sensitivity;
    public float looklimit;
    [Space]
    [Header("External Config")]
    [SerializeField] private Vector3 ForceToApply;
    [SerializeField] private float ForceDamping;
    [SerializeField] private LayerMask grounded_mask;

    private CharacterController CharacterController;
    private Vector3 moveDirection;
    private Vector2 rotation, baseInput;

    [HideInInspector] public bool canMove = true;
    private bool hasjumped = false;

    // Start is called before the first frame update
    void Start()
    {
        CharacterController = GetComponent<CharacterController>();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        rotation.y = transform.eulerAngles.y;
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 400, 30), $"Player's Forward {transform.forward}, Right : {transform.right}");
        GUI.Label(new Rect(10, 30, 400, 30), $"Player's MovingDistance = {moveDirection}");
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;
        baseInput.x = Input.GetAxis("Horizontal") * player_speed; //Right
        baseInput.y = Input.GetAxis("Vertical") * player_speed; //Forward
        if (IsGrounded())
        {
            #region Move
            moveDirection = baseInput.x * transform.right + baseInput.y * transform.forward;
            #endregion
            if (Input.GetButtonDown("Jump") && !hasjumped)
            {
                hasjumped = true;
                moveDirection.y = jump_power;
                Invoke("ResetJump", resettime);
            }
        }
        else
        {
            #region Move + AirForce
            moveDirection += (baseInput.x * transform.right + baseInput.y * transform.forward) * airmultiplier;
            #endregion
        }
        //not effectively walking
        if (Mathf.Sign(baseInput.y) < 0 || Mathf.Abs(baseInput.x) > 0.2f)
        {
            moveDirection.x *= 0.7f;
            moveDirection.z *= 0.7f;
        }
        //to normalize speed (we dont normalize the input so that it doesnt feel clicky / terlalu kaku
        moveDirection = Vector3.ClampMagnitude(new Vector3(moveDirection.x, 0, moveDirection.z), player_speed) + Vector3.up * moveDirection.y;

        moveDirection += ForceToApply;
        ForceToApply = Vector3.Lerp(ForceToApply, Vector3.zero, ForceDamping * Time.deltaTime);

        //Apply full stop on certain value
        if (new Vector2(moveDirection.x, moveDirection.z).magnitude < friction)
        {
            moveDirection = new Vector3(0, moveDirection.y, 0);
        }

        //Apply gravity (gravity is acceleration [M][T][T])
        moveDirection.y -= gravity * Time.deltaTime;
        CharacterController.Move(moveDirection * Time.deltaTime);
        //#region Player and Camera Rotation
        //if (canMove)
        //{
        //    rotation.y += Input.GetAxis("Mouse X") * camera_sensitivity;
        //    rotation.x += -Input.GetAxis("Mouse Y") * camera_sensitivity;
        //    rotation.x = Mathf.Clamp(rotation.x, -looklimit, looklimit);
        //    playercam.transform.localRotation = Quaternion.Euler(rotation.x, 0, 0);
        //    transform.eulerAngles = new Vector2(0, rotation.y);
        //}
        //#endregion
    }
    public void ResetJump()
    {
        hasjumped = false;
    }
    public bool IsGrounded()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, new Vector3(0, -1, 0), out hit, 100, grounded_mask);
        if (hit.distance == 0 || hit.distance > 1.08f)
        {
            return false;
        }
        else return true;
    }
}
