using System;
using UnityEngine;

public interface IEntity
{
    public void ReceiveDamage(float damage);
}
public enum Damage_type
{
    trap, weapon
}
public class 
public class Entity : MonoBehaviour, IEntity
{
    protected float MaxHealth;
    protected float Health;
    private float _Health
    {
        get { return Health; }
        set
        {
            Health = value;
            if(Health <= 0)
            {
                OnHP0();
            }
        }
    }

    protected float MaxMana;
    protected float Mana;

    protected float MaxStamina;
    protected float Stamina;

    protected float Speed;

    protected float Hunger;
    protected float MaxHunger;

    protected float defense;
    //nanti ganti ke agi str int
    public virtual void OnHP0()
    {
        Destroy(this);
    }
    public virtual void ReceiveDamage(float damage)
    {
        float damageReceived = Mathf.Clamp(damage - defense, 1, damage);
        _Health -= damageReceived;
    }
}
