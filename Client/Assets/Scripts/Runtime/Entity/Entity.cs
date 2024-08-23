using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public delegate void ValueChangedHandler(float currentValue, float maxValue);

    public ValueChangedHandler HpChanged;
    
    public int Id { get; set; }
    public Define.EntityType Type { get; set; }
    public GameObject Model { get; set; }
    public Define.EntityState State { get; set; }

    private List<Vector3> _path = new List<Vector3>();

    private void Awake()
    {
    }

    private IEnumerator CoMove()
    {
        foreach (Vector3 vector3 in _path)
        {
            yield return new WaitForSeconds(0.1f);
            transform.position = vector3;
            Debug.Log(vector3);
        }
    }
    
    public void Move(List<Vector3> position)
    {
        StopCoroutine(CoMove());
        
        _path = position;
        
        StartCoroutine(CoMove());
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
