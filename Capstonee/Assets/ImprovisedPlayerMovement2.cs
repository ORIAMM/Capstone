using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;
public class ImprovisedPlayerMovement2 : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float player_speed;
    [SerializeField] private float rotSmoothTime;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 5f;
    [SerializeField] private float DodgeCooltime;

    [Header("Friction Settings")]
    [SerializeField] private float dampingValue;

    [HideInInspector] public Player2 core;
    [HideInInspector] public Transform player;
    [HideInInspector] public Transform FacingDirection;
    [HideInInspector] public Transform camerapos;
    [HideInInspector] public float ExteriordampingValue;

    Vector3 moveDirection = Vector3.zero, right, forward;
    private float rotSpeed;
    private CharacterController controller;
    private Animator animator;

    //[HideInInspector] public Coroutine isDodging;
    //float dodgeTime = 0;
    //bool dodgeCooldown => Time.time >= dodgeTime;
    float DampingValue => dampingValue + ExteriordampingValue;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
    public void Move(Vector2 input)
    {
        Vector2 baseInput = new(input.x * player_speed, input.y * player_speed);
        right = baseInput.x * FacingDirection.right;
        forward = baseInput.y * FacingDirection.forward;
        moveDirection += new Vector3(right.x, 0, right.z);
        moveDirection += new Vector3(forward.x, 0, forward.z);
        if (baseInput.magnitude > 0f && !core.isCombat)
        {
            float targetAngle = Mathf.Atan2(baseInput.x, baseInput.y) * Mathf.Rad2Deg + camerapos.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(player.eulerAngles.y, targetAngle, ref rotSpeed, rotSmoothTime);
            player.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }
    public void ApplyMove(float gravity)
    {
        float yMagnitude = moveDirection.y;
        moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
        moveDirection = Vector3.Slerp(moveDirection, Vector3.zero, Time.deltaTime * DampingValue);
        moveDirection = Vector3.ClampMagnitude(moveDirection, player_speed) + Vector3.up * yMagnitude;
        controller.Move(moveDirection * Time.deltaTime);
        Vector3 velValue = new(controller.velocity.x, 0, controller.velocity.z);
        Vector3 forwardVel = new(FacingDirection.forward.x, 0, FacingDirection.forward.z);
        Vector3 rightVel = new(FacingDirection.right.x, 0, FacingDirection.right.z);
        moveDirection.y -= gravity * Time.deltaTime;

        if (velValue.magnitude > 0.01f)
        {
            animator.SetFloat("VelX", Vector3.Dot(rightVel, velValue) / player_speed);
            animator.SetFloat("VelY", Vector3.Dot(forwardVel, velValue) / player_speed);
        }
    }
    public void ApplyMove(float gravity, bool noClamps = false)
    {
        float yMagnitude = moveDirection.y;
        if (!noClamps)
        {
            moveDirection = Vector3.Slerp(moveDirection, Vector3.zero, Time.deltaTime * DampingValue);
            moveDirection = Vector3.ClampMagnitude(new Vector3(moveDirection.x, 0, moveDirection.z), player_speed) + Vector3.up * yMagnitude;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    public void Dash(Vector2 DodgeInput)
    {
        if(DodgeInput.magnitude == 0)
        {
            moveDirection = player.forward * dashSpeed;
        }
        else
        {
            moveDirection = FacingDirection.TransformDirection(DodgeInput.x, 0, DodgeInput.y) * dashSpeed;
        }
    }
}