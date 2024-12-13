using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class Player2 : MonoBehaviour, IEntity
{
    private Animator _animator;
    private CharacterController controller;

    [Header("Main Settings")]
    [SerializeField] private ImprovisedPlayerMovement2 movement;
    [SerializeField] private PlayerCamera2 playerCam;
    [SerializeField] private CombatHandler playerCombat;
    [SerializeField] private PlayerInput PI;

    [Header("Player Stat")]
    [SerializeField] private float initial_time = 300f;
    [SerializeField] private float DmgReduct = 0.5f;
    public float HealthPlayer;
    public float TempHealth;

    [Header("References")]
    [SerializeField] private Transform CameraTransform;
    [SerializeField] private Transform PlayerMeshObject;
    [SerializeField] private Transform FacingDirection;

    [SerializeField] private float Gravity = 9.81f;

    [HideInInspector] public Slider HealthSlider;
    [HideInInspector] public TextMeshProUGUI time;
    [HideInInspector] public GameObject DeathPanel;

    public CameraStyle _CameraStyle;
    [NonSerialized] public float modifierMultiplier = 1;
    public bool isCombat => playerCam.target != null;
    public bool Dodging => playerCombat.isDodging;
    private bool isDead = false;

    private PlayerInput pi;
    private InputActionAsset _actionAsset;
    private InputActionMap InputAction;
    private PlayerInput playerInput;
    private void Awake()
    {
        Cursor.visible = false;
        _actionAsset = GetComponent<PlayerInput>().actions;
        InputAction = _actionAsset.FindActionMap("Player");
        pi = GetComponent<PlayerInput>();   

        _animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        playerCam = GetComponent<PlayerCamera2>();
        playerCombat = GetComponent<CombatHandler>();

        //Initialize initial_Health
        HealthPlayer = Time.time + initial_time;

        TempHealth = HealthPlayer;
        if (HealthSlider != null)
        {
            HealthSlider.value = TempHealth;
            HealthSlider.maxValue = HealthPlayer;
            HealthSlider.minValue = Time.time;
        }
        else Debug.LogError("GK ADA");

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
        if (TimeManager.instance.OnCooldown == null && TimeManager.instance.Skillready && playerCombat.isFall == false)
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
        TempHealth -= Time.deltaTime;
        HealthSlider.value = TempHealth;
        if (TempHealth >= 0)
        {
            FloatToTimeConverse();
        }
    }
    public void FloatToTimeConverse()
    {
        int totalSeconds = Mathf.CeilToInt(TempHealth - Time.time); // Pembulatan ke atas
        int minutes = totalSeconds / 60; // Hitung menit
        int seconds = totalSeconds % 60; // Hitung detik

        time.text = $"{minutes:00}:{seconds:00}"; // Format MM:SS
    }

    public void ReceiveDamage(float value)
    {
        if (playerCombat.isFall == true || playerCombat.isDodging == true || isDead == true) return;

        Debug.Log("receive");
        if (playerCombat.isBlocking == true)
        {
            Debug.Log("Blocked");
            playerCombat.Impact();
            HealthPlayer -= value * DmgReduct;
            TempHealth -= value * DmgReduct;
        }
        else
        {
            Debug.Log("Fall");
            playerCombat.OnHit();
            HealthPlayer -= value;
            TempHealth -= value;   
        }

    }
    public void OnDeath()
    {
        if (isDead == false)
        {
            isDead = true;
            playerCombat.enabled = false;
            playerCam.enabled = false;
            movement.enabled = false;
            //PI.enabled = false;
            StartCoroutine(AfterDeath());
        }
    }
    IEnumerator AfterDeath()
    {
        _animator.SetTrigger("Die");
        Cursor.visible = true;
        var CGdeath = DeathPanel.GetComponent<CanvasGroup>();
        CGdeath.DOFade(1, 0.5f);
        //pi.enabled = false;
        UIManage.instance.Deathcount += 1;
        yield return new WaitForSeconds(0.5f);
        UIManage.instance.DeathCheck();
        enabled = false;
    }
    Vector2 MoveValue => InputAction.FindAction("Move").ReadValue<Vector2>();
}
