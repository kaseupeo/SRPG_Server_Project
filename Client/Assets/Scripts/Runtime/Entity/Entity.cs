using System;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public delegate void ValueChangedHandler(float currentValue, float maxValue);

    public ValueChangedHandler HpChanged;
    
    public int Id { get; set; }
    public GameObject Model { get; set; }
    public Define.EntityState State { get; set; }

    private void Awake()
    {
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
    
    public void TakeDamage(int maxHp, int hp, int damage)
    {
        HpChanged.Invoke(hp, maxHp);
        Debug.Log($"{Id} - Take Damage, Hp : {hp}");
    }

    public void Dead()
    {
        HpChanged = null;
        Debug.Log($"Id : {Id}");
        Destroy(gameObject);
    }
}
