using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputManage : MonoBehaviour
{
    private Player _Player;
    private PlayerInput _PlayerInput;
    private PlayerCamera _PlayerCamera;
    public Vector2 MoveValue
    {
        get;
        private set;
    }
    public Vector2 MouseValue
    {
        get;
        private set;
    }

    #region optional
    [HideInInspector] public bool Dodged = false;
    [HideInInspector] public bool Attacked = false;
    [HideInInspector] public bool Blocked = false;
    [HideInInspector] public bool Escaped = false;
    [HideInInspector] public bool Skill = false;
    [HideInInspector] public bool Target = false;
    #endregion

    private void Awake()
    {
        _PlayerInput = GetComponent<PlayerInput>();
        var players = FindObjectsOfType<Player>();
        var cameras = FindObjectsOfType<PlayerCamera>();
        var index = _PlayerInput.playerIndex;
        _Player = players.FirstOrDefault(p => p.GetIndex() == index);
        _PlayerCamera = cameras.FirstOrDefault(m => m.GetIndex() == index);
    }
    public void OnMove(CallbackContext context)
    {
        if(_Player != null & _PlayerCamera != null)
        {
            _Player.MoveValue = context.ReadValue<Vector2>();
            _PlayerCamera.Input = context.ReadValue<Vector2>();
        }
    }
    public void OnDodge(CallbackContext context)
    {
        Dodged = context.action.triggered;
    }
    public void Mouse(CallbackContext context)
    {
        //MouseValue = context.ReadValue<Vector2>();
        _PlayerCamera.MouseInput = context.ReadValue<Vector2>();
    }
    public void Attack(CallbackContext context)
    {
        if (context.action.triggered)
        {
            Debug.Log("hei");

        }
    }
    public void Block(CallbackContext context)
    {
        Blocked = context.action.triggered;
    }
    public void Escape(CallbackContext context)
    {
        Escaped = context.action.triggered;
    }
    public void OnSkill(CallbackContext context)
    {
        Skill = context.action.triggered;
    }
    public void OnTarget(CallbackContext context)
    {
        if (context.action.triggered)
        {
            Debug.Log("cek");
            _Player.Targeting();
        }
    }
}
