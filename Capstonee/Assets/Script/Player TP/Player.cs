using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Windows;

public enum CameraStyle
{
    Basic, Combat
}

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private Animator _animator;
    private PlayerControls input;
    private CharacterController controller;
    [Header("Main Settings")]
    [SerializeField] private ImprovisedPlayerMovement movement;
    [SerializeField] private PlayerCamera playerCam;
    [SerializeField] private PlayerCombat playerCombat;

    [Header("References")]
    [SerializeField] private Transform PlayerMeshObject;
    [SerializeField] private Transform FacingDirection;

    [SerializeField] private float Gravity = 9.81f;
    private void OnEnable()
    {
        input.Movement.Enable();
        input.Controls.Enable();
    }
    private void OnDisable()
    {
        input.Movement.Disable();
        input.Controls.Disable();
    }
    private void Awake()
    {
        input = new();
        _animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        //Initialize Movement
        movement.camerapos = Camera.main.transform;
        movement.player = PlayerMeshObject;
        movement.FacingDirection = FacingDirection;
        movement.Set(controller, _animator);

        //Initialize Camera
        playerCam.PlayerMeshObject = PlayerMeshObject;
        playerCam.FacingDirection = FacingDirection;   
        playerCam.input = input;

        input.Controls.Target.performed += (val) => playerCam.GetTarget();
        input.Movement.Jump.performed += (val) => movement.Jump();
    }
    private void Update()
    {
        movement.Move(MoveValue);
        movement.ApplyMove(Gravity);
    }
    Vector2 MoveValue => input.Movement.Move.ReadValue<Vector2>();
}
