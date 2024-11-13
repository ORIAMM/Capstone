using System;
using UnityEngine;

public enum Damage_type
{
    trap, weapon
}
public enum Race_type
{
    Human, Dwarf, Elf, Lizardmen, Orc, Goblin, Undead
}
public class Entity : MonoBehaviour
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
    public virtual void ReceiveDamage(float ammmount)
    {
        float damageReceived = Mathf.Clamp(ammmount - defense, 1, ammmount);
        _Health -= damageReceived;
    }
    public virtual bool UseMana(float ammount)
    {
        if(Mana <= ammount)return true;
        Mana -= ammount;
        return false;
    }
}
