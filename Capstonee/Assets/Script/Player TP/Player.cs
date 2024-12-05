using System.Collections;
using TMPro;
using UnityEngine;
public enum CameraStyle
{
    Basic, Combat
}
public interface IEntity
{
    public void ReceiveDamage(float value);
    public void OnDeath();
}

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour, IEntity
{
    private Animator _animator;
    private PlayerControls input;
    private CharacterController controller;

    [Header("Main Settings")]
    [SerializeField] private ImprovisedPlayerMovement movement;
    [SerializeField] private PlayerCamera playerCam;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private UI_Controller uI_Controller;

    [Header("Player Stat")]
    [SerializeField] private float initial_time = 300f;
    [SerializeField] private float DmgReduct = 0.5f;
    public float HealthPlayer;
    
    [Header("References")]
    [SerializeField] private Transform PlayerMeshObject;
    [SerializeField] private Transform FacingDirection;

    [SerializeField] private float Gravity = 9.81f;

    public CameraStyle _CameraStyle;
    private TimeManager timeManager;
    public bool isCombat => playerCam.target != null;
    public bool Dodging => movement.isDodging != null;
    private void OnEnable()
    {
        input.Movement.Enable();
        input.Controls.Enable();
        StartTicking();
    }
    private void OnDisable()
    {
        input.Movement.Disable();
        input.Controls.Disable();
    }
    private void Awake()
    {
        Cursor.visible = false;
        input = new();
        _animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        timeManager = GetComponent<TimeManager>();

        playerCam = GetComponent<PlayerCamera>();
        playerCombat = GetComponent<PlayerCombat>();

        //Initialize initial_Health
        HealthPlayer = Time.time + initial_time;

        //Initialize Movement
        movement.camerapos = Camera.main.transform;
        movement.player = PlayerMeshObject;
        movement.FacingDirection = FacingDirection;
        movement.Set(controller, _animator);
        movement.core = this;

        //Initialize Camera
        playerCam.PlayerMeshObject = PlayerMeshObject;
        playerCam.FacingDirection = FacingDirection; 
        playerCam.input = input;
        playerCam.animator = _animator;
        playerCam._CameraStyle = _CameraStyle;

        //Initialize Combat
        playerCombat.animator = _animator;
        timeManager.animator = _animator;
        playerCombat.player =  PlayerMeshObject;

        //Initialize Input
        input.Controls.Target.performed += (val) => playerCam.GetTarget();
        input.Controls.Attack.performed += (val) => playerCombat.Attack();
        input.Controls.Block.performed += (val) => playerCombat.Block();
        input.Controls.Skill.performed += (val) => timeManager.UseSkill(5f);
        input.Movement.Dodge.performed += (val) => playerCombat.Dodge(MoveValue);
        //input.Controls.Escape.performed += (val) => uI_Controller.PauseGame();

        //Debug Animation
        input.Controls.Test.performed += (val) => ReceiveDamage(5);

        //input.Movement.Jump.performed += (val) => movement.Jump();
        //input.Movement.Dodge.performed += (val) => movement.Dodge();
    }
    private void Update()
    {
        if (HealthPlayer > 0)
        {
            FloatToTimeConverse(); // Perbarui timer UI
        }

        if (playerCombat.isBlocking == false && playerCombat.isFall == false && playerCombat.isDodging == false)
        {
            Vector2 adjustedMoveValue = MoveValue;
            switch (_CameraStyle)
            {
                case CameraStyle.Basic:
                    movement.Move(adjustedMoveValue);
                    movement.ApplyMove(Gravity);
                    break;
                case CameraStyle.Combat:
                    adjustedMoveValue *= 0.5f;
                    if (playerCombat.isAttacking == true)
                    {
                        adjustedMoveValue *= 0.05f;
                    }
                    movement.Move(adjustedMoveValue);
                    movement.ApplyMove(Gravity);
                    break;
            }
        }
    }
    public void FloatToTimeConverse()
    {
        int totalSeconds = Mathf.CeilToInt(HealthPlayer); // Pembulatan ke atas
        int minutes = totalSeconds / 60; // Hitung menit
        int seconds = totalSeconds % 60; // Hitung detik

        TimerText.text = $"{minutes:00}:{seconds:00}"; // Format MM:SS
    }

    public TextMeshProUGUI TimerText;
    private Coroutine tickingCoroutine;
    public void StartTicking()
    {
        if (tickingCoroutine == null)
        {
            tickingCoroutine = StartCoroutine(TickingCoroutine());
        }
    }
    private IEnumerator TickingCoroutine()
    {
        while (HealthPlayer > 0)
        {
            yield return new WaitForSeconds(1f); // Kurangi setiap detik
            HealthPlayer--; // Kurangi HealthPlayer setiap detik
            FloatToTimeConverse(); // Update Timer
        }

        HealthPlayer = 0; // Pastikan tidak negatif
        OnDeath(); // Panggil jika waktu habis
    }
    public void ReceiveDamage(float value)
    {
        if (playerCombat.isFall == true || playerCombat.isDodging == true) return;
        else
        {
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
                playerCombat.Interrupt();
                HealthPlayer -= value;
            }
        }
    }
    public void OnDeath()
    {
        SoundManager.instance.StopAllMusic();
        SoundManager.instance.StopAllSFX();

        _animator.SetTrigger("Death");
        OnDisable();

        if (tickingCoroutine != null)
        {
            StopCoroutine(tickingCoroutine);
            tickingCoroutine = null;
        }

        uI_Controller.isLose = true;
        uI_Controller.Defeat();
    }
    Vector2 MoveValue => input.Movement.Move.ReadValue<Vector2>();

    public UI_Controller ui;
}
