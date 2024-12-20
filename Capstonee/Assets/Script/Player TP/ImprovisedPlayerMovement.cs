﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImprovisedPlayerMovement : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private float JumpPower;
    [SerializeField] private float player_speed;
    [SerializeField] private float rotSmoothTime;
    [SerializeField] private float DodgeCooltime;

    [Header("Friction Settings")]
    [SerializeField] private float dampingValue;

    [HideInInspector] public Player core;
    [HideInInspector] public Transform player;
    [HideInInspector] public Transform FacingDirection;
    [HideInInspector] public Transform camerapos;
    [HideInInspector] public float ExteriordampingValue;
    CharacterController controller;

    public Animator animator;
    Vector3 moveDirection = Vector3.zero, right, forward;
    private float rotSpeed;
    [HideInInspector] public Coroutine isDodging;
    float dodgeTime = 0;
    bool dodgeCooldown => Time.time >= dodgeTime;
    float DampingValue => dampingValue + ExteriordampingValue;
    public void Set(CharacterController controller, Animator animator)
    {
        this.controller = controller;
        this.animator = animator;
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
        moveDirection = Vector3.Slerp(moveDirection, Vector3.zero, Time.deltaTime * DampingValue);
        moveDirection = Vector3.ClampMagnitude(new Vector3(moveDirection.x, 0, moveDirection.z), player_speed) + Vector3.up * yMagnitude;
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
    public void ApplyMove(bool noClamps = false)
    {
        float yMagnitude = moveDirection.y;
        if (!noClamps)
        {
            moveDirection = Vector3.Slerp(moveDirection, Vector3.zero, Time.deltaTime * DampingValue);
            moveDirection = Vector3.ClampMagnitude(new Vector3(moveDirection.x, 0, moveDirection.z), player_speed) + Vector3.up * yMagnitude;
        }
        controller.Move(moveDirection * Time.deltaTime);
    }

    public void Dash(Vector2 DodgeInput)
    {
        Vector2 direction = DodgeInput.magnitude == 0 ? player.forward : player.TransformDirection(DodgeInput);
        float dashSpeed = 5f; // Adjust the dash speed as needed
        Vector3 dashDirection = direction * dashSpeed;

        // Apply movement
        controller.Move(dashDirection * Time.deltaTime);
    }

}
