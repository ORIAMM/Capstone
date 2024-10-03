using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform Enemy_Target;
    [SerializeField] private float update_time;
    [SerializeField] private NavMeshAgent agent;
    public enum state_event
    {
        dies
    }
    public enum state
    {
        Idle,
        Patrol,
        Chase,
        Shoot,
        Cover,
        Dash
    }
    public StateMachine<state, state_event> SM = new StateMachine<state, state_event>();
    private bool inRange;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        SM.AddnewState(state.Idle, new Enemy_Idle());
        SM.AddnewState(state.Chase, new Enemy_Chase(Enemy_Target, update_time, agent));
        SM.AddTransition(new Transition<state>(state.Idle, state.Chase, condition => inRange));
        SM.setState(state.Idle);
        SM.OnEnter();
    }

    // Update is called once per frame
    void Update()
    {
        SM.OnLogic();
        Debug.Log(SM.current_state);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("Player found");
            inRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Player gone");
            inRange = false;
        }
    }
}
