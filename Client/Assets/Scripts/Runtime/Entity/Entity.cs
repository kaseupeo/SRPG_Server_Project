using System;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int Id { get; set; }
    public GameObject Model { get; set; }
    public Define.EntityState State { get; set; }
    public Stats Stats { get; set; }

    private void Awake()
    {
        Stats = gameObject.GetOrAddComponent<Stats>();
    }

    public void Move(List<Vector3> position)
    {
        // TODO : 임시 
        transform.position = position[0];
    }

    public void Attack(int damage)
    {
        // TODO :
        Debug.Log($"{Id} - Attack, Damage : {damage}");
        State = Define.EntityState.EndTurn;
    }
    
    public void TakeDamage(int hp, int damage)
    {
        Debug.Log($"{Id} - Take Damage, Hp : {hp}");
        // TODO : 
        if (hp <= 0) 
            Dead();
    }

    public void Dead()
    {
        // TODO :
    }
}
