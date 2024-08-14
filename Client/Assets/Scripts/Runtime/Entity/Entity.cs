using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int ID { get; set; }
    public Stats Stats { get; set; }

    private void Awake()
    {
        Stats = gameObject.AddComponent<Stats>();
        
    }
}
