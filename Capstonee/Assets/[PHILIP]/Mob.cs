using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

//Kroco yah
//1. Kejer (map kecil gada pathfinding) gampangnya ambil arah ke player terus velocity x z nya ikut player
//2. Kalo dah deket pukul, ini bisa pake distance, pukul pake transform.forward overlap

public abstract class AttackInfo 
{
    public float distance;
    public abstract void AttackBehaviour();
}
public class Mob_BasicAttack : AttackInfo
{
    public override void AttackBehaviour()
    {

    }
}
public class Mob : TimedObject, IEntity
{
    [SerializeField] private float Health;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float ChaseRadius;
    [SerializeField] private float DampingValue;


    [SerializeField] private LayerMask playerLayer;
    private List<AttackInfo> attackInfos = new();
    private CharacterController controller;
    private Animator animator;

    private Transform player;

    Vector3 moveDirection = Vector3.zero;
    private AttackInfo currentAttackQueue;
    public void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
        AddInfos(new()
        {
            new Mob_BasicAttack()
        });
    }
    protected override void Update()
    {
        base.Update();
        Target();
        if (player)
        {
            LookToPlayer();
            GetForwardMove();
        }
        ApplyMovement();
    }
    public void LookToPlayer()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    public void GetForwardMove()
    {
        var forward = movementSpeed * transform.forward;
        moveDirection += new Vector3(forward.x, 0, forward.z);
    }
    public void Target()
    {
        float closestDistance = ChaseRadius + 1;
        Collider player = null;
        List<Collider> players = Physics.OverlapSphere(transform.position, ChaseRadius, playerLayer).ToList();
        foreach(Collider ply in players)
        {
            float distance = Vector3.Distance(ply.transform.position, transform.position);
            if (closestDistance > distance)
            {
                player = ply;
                closestDistance = distance;
            }
        }
        if (!player) return;
        else this.player = player.transform;
    }
    public void ApplyMovement()
    {
        float yMagnitude = moveDirection.y;
        moveDirection = Vector3.Slerp(moveDirection, Vector3.zero, Time.deltaTime * DampingValue);
        moveDirection = Vector3.ClampMagnitude(new Vector3(moveDirection.x, 0, moveDirection.z), movementSpeed) + Vector3.up * yMagnitude;
        controller.Move(moveDirection * Time.deltaTime);
        Vector3 velValue = new(controller.velocity.x, 0, controller.velocity.z);
        Vector3 forwardVel = new(transform.forward.x, 0, transform.forward.z);
        Vector3 rightVel = new(transform.right.x, 0, transform.right.z);
        moveDirection.y -= gravity * Time.deltaTime;

        if (velValue.magnitude > 0.01f)
        {
            animator.SetFloat("VelX", Vector3.Dot(rightVel, velValue) / movementSpeed);
            animator.SetFloat("VelY", Vector3.Dot(forwardVel, velValue) / movementSpeed);
        }
    }
    public override void OnContinue()
    {
    }
    public override void OnStop()
    {
    }
    public void OnDeath()
    {
    }
    public void ReceiveDamage(float value)
    {
    }
    public void AddInfos(List<AttackInfo> infos)
    {
        foreach (AttackInfo info in infos) attackInfos.Add(info);
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, ChaseRadius);
    }
#endif
}
