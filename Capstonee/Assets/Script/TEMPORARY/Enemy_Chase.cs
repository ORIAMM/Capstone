using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Chase : BaseState
{
    private float Update_Time;
    private float current_time;
    private Transform Enemy_target;
    private NavMeshAgent agent;

    public Enemy_Chase(Transform target, float value, NavMeshAgent agent){
        Update_Time = value;
        Enemy_target = target;
        this.agent = agent;
    }
    public override void OnEnter()
    {
        current_time = Time.time + Update_Time;
    }
    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }
    public override void OnLogic()
    {
        Debug.Log("chasing");
        if(Time.time > current_time)
        {
            current_time = Time.time + Update_Time;
            agent.SetDestination(Enemy_target.position);
        }
    }

}
