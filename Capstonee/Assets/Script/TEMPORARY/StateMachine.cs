using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachine<State, StateEvent> where State : Enum where StateEvent : Enum 
{
    State stated;
    public BaseState current_state;
    public Dictionary<State, BaseState> states = new Dictionary<State, BaseState>();
    public Dictionary<(StateEvent, BaseState), BaseState> transitions = new Dictionary<(StateEvent, BaseState), BaseState>();
    public Dictionary<BaseState, List<Transition<State>>> conditions = new Dictionary<BaseState, List<Transition<State>>>();
    public void OnEnter()
    {
        current_state.OnEnter();
    }
    public void OnLogic()
    {
        current_state.OnLogic();
        check();
    }
    public void setState(State state)
    {
        current_state = states[state];
    }
    public void AddnewState(State state, BaseState baseState)
    {
        states.Add(state, baseState);
    }
    /// <summary>
    /// Add a transition with StateEvent as the event detector, State current position, Other State change
    /// </summary>
    /// <param name="State Event"></param>
    public void AddTriggerTransition(StateEvent sevent, BaseState current_state, BaseState next_state)
    {
        transitions.Add((sevent, current_state), next_state);
    }
    /// <summary>
    /// Find any transitions
    /// </summary>
    /// <param name="sevent"></param>
    public void Trigger(StateEvent sevent)
    {
        BaseState next_state = transitions[(sevent, current_state)];
    }
    public void AddTransition(Transition<State> transition)
    {
        if (!conditions.ContainsKey(states[transition.origin]))
        {
            conditions.Add(states[transition.origin], new List<Transition<State>>());
        }
        conditions[states[transition.origin]].Add(transition);
        Debug.Log(states[transition.origin]);
    }
    public void check()
    {
        if (conditions.ContainsKey(current_state))
        {
            foreach (Transition<State> st in conditions[current_state])
            {
                Debug.Log(st.checkcondition());
                if (st.checkcondition())
                {
                    current_state.OnExit();
                    current_state = states[st.next];
                    current_state.OnEnter();
                    break;
                }
            }
        }
    }
    //public void AddCondition(BaseState current_state, BaseState next_state, bool condition)
    //{
    //    if (!conditions.ContainsKey(current_state))
    //    {
    //        conditions.Add(current_state, new List<(bool, BaseState)>());
    //    }
    //    conditions[current_state].Add((condition, next_state));
    //}
    //public void AddCondition(BaseState current_state, BaseState next_state, bool condition, int priority)
    //{
    //    if (!conditions.ContainsKey(current_state))
    //    {
    //        conditions.Add(current_state, new List<(bool, BaseState)>());
    //    }
    //    else
    //    {
    //        conditions[current_state].Insert(priority,(condition, next_state));
    //    }
    //}

}

public class Transition<State> where State : Enum
{
    Func<Transition<State>, bool> condition;
    public State origin;
    public State next;
    public Transition(
        State from,
        State to,
        Func<Transition<State>, bool> condition = null
        )
    {
        this.condition = condition;
        origin = from;
        next = to;
    }
    
    public bool checkcondition()
    {
        if (condition == null) return false;
        return condition(this);
    }
}
