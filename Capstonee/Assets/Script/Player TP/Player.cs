using System.Collections;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Windows;

public enum CameraStyle
{
    Basic, Combat
}
public interface IEntity
{
    public void ReceiveDamage(float value);
    public void OnDeath();
}
public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    public bool isStopped;
    private void Awake()
    {
        instance = this;
    }
    public IEnumerator StopTime(float Time)
    {
        isStopped = true;
        yield return new WaitForSeconds(Time);
        isStopped = false;
    }
}
public class TimedObject : MonoBehaviour
{
    public Coroutine TimeStopped;
    private void Update()
    {
        if (TimeManager.instance.isStopped || TimeStopped != null) TimeStopped ??= StartCoroutine(OnStop());
    }
    public virtual IEnumerator OnStop()
    {
        yield return new WaitUntil(() => TimeManager.instance.isStopped == false);
        OnContinue();
        TimeStopped = null;
    }
    public virtual void OnContinue()
    {

    }
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

    [Header("References")]
    [SerializeField] private Transform PlayerMeshObject;
    [SerializeField] private Transform FacingDirection;

    [SerializeField] private float Gravity = 9.81f;
    public bool isCombat => playerCam.target != null;
    public bool Dodging => movement.isDodging != null;
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
        movement.core = this;

        //Initialize Camera
        playerCam.PlayerMeshObject = PlayerMeshObject;
        playerCam.FacingDirection = FacingDirection; 
        playerCam.input = input;
        playerCam.animator = _animator;

        //Initializa Combat
        playerCombat.animator = _animator;

        input.Controls.Target.performed += (val) => playerCam.GetTarget();
        input.Controls.Attack.performed += (val) => playerCombat.Attack();
        input.Controls.Block.performed += (val) => playerCombat.Block();
        //input.Movement.Jump.performed += (val) => movement.Jump();
        input.Movement.Dodge.performed += (val) => movement.Dodge();
    }
    private void Update()
    {
        Debug.Log(playerCombat.coroutine == null);
        if (playerCombat.isBlocking == false)
        {
            Vector2 adjustedMoveValue = MoveValue;
            if (playerCombat.isAttacking == true)
            {
                adjustedMoveValue *= 0.07f;
            } 
            movement.Move(adjustedMoveValue);
            movement.ApplyMove(Gravity);

        }

    }
    public void ReceiveDamage(float value)
    {
        if (!Dodging)
        {

        }
    }
    public void OnDeath()
    {
        throw new System.NotImplementedException();
    }
    Vector2 MoveValue => input.Movement.Move.ReadValue<Vector2>();
}
