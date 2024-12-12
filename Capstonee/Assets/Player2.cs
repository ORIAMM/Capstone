using UnityEngine;
using System;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player2 : MonoBehaviour, IEntity
{
    private Animator _animator;
    private CharacterController controller;

    [Header("Main Settings")]
    [SerializeField] private ImprovisedPlayerMovement2 movement;
    [SerializeField] private PlayerCamera2 playerCam;
    [SerializeField] private CombatHandler playerCombat;

    [Header("Player Stat")]
    [SerializeField] private float initial_time = 300f;
    [SerializeField] private float DmgReduct = 0.5f;
    public float HealthPlayer;

    [Header("References")]
    [SerializeField] private Transform CameraTransform;
    [SerializeField] private Transform PlayerMeshObject;
    [SerializeField] private Transform FacingDirection;

    [SerializeField] private float Gravity = 9.81f;

    public CameraStyle _CameraStyle;
    [NonSerialized] public float modifierMultiplier = 1;
    public bool isCombat => playerCam.target != null;
    public bool Dodging => playerCombat.isDodging;
    private InputActionAsset _actionAsset;
    private InputActionMap InputAction;
    private PlayerInput playerInput;
    private void Awake()
    {
        Cursor.visible = false;
        _actionAsset = GetComponent<PlayerInput>().actions;
        InputAction = _actionAsset.FindActionMap("Player");

        _animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        playerCam = GetComponent<PlayerCamera2>();
        playerCombat = GetComponent<CombatHandler>();

        //Initialize initial_Health
        HealthPlayer = Time.time + initial_time;

        //Initialize Movement
        movement.camerapos = CameraTransform;
        movement.player = PlayerMeshObject;
        movement.FacingDirection = FacingDirection;
        movement.core = this;

        //Initialize Camera
        playerCam.PlayerMeshObject = PlayerMeshObject;
        playerCam.FacingDirection = FacingDirection;
        playerCam.animator = _animator;
        playerCam.action = InputAction;

        //Initialize Combat
        playerCombat.animator = _animator;
        playerCombat.player = PlayerMeshObject;

        InputAction.FindAction("Target").performed += (val) => playerCam.GetTarget();
        InputAction.FindAction("Attack").performed += (val) =>
        {
            if (_CameraStyle == CameraStyle.Combat) playerCombat.Attack();
        };
        InputAction.FindAction("Block").performed += (val) => playerCombat.Block();
        InputAction.FindAction("Skill").performed += (val) => OnSkill();
        InputAction.FindAction("Dodge").performed += (val) => playerCombat.Dodge(MoveValue);

        //Debug Animation
        //input.Controls.Test.performed += (val) => ReceiveDamage(5);
    }
    public void OnSkill()
    {
        if (TimeManager.instance.OnCooldown == null)
        {
            _animator.SetTrigger("Skill");
            TimeManager.instance.UseSkill(5f);
        }
    }

    private void Update()
    {
        if (!playerCombat.isBlocking && !playerCombat.isFall && !playerCombat.isDodging)
        {
            movement.Move(MoveValue * modifierMultiplier * (playerCombat.isAttacking ? 0.05f : 1));
        }
        if (playerCombat.isDodging) movement.ApplyMove(Gravity, true);
        else movement.ApplyMove(Gravity);
        Ticking();
    }

    public void Ticking()
    {
        if (HealthPlayer - Time.time <= 0)
        {
            OnDeath();
        }
    }

    public void ReceiveDamage(float value)
    {
        if (playerCombat.isFall == true || playerCombat.isDodging == true) return;

        Debug.Log("receive");
        if (playerCombat.isBlocking == true)
        {
            Debug.Log("Blocked");
            playerCombat.Impact();
            HealthPlayer -= value * DmgReduct;
        }
        else
        {
            Debug.Log("Fall");
            playerCombat.OnHit();
            HealthPlayer -= value;
        }

    }
    public void OnDeath()
    {
        Debug.Log("Mati");
    }
    Vector2 MoveValue => InputAction.FindAction("Move").ReadValue<Vector2>();
}
