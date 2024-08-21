using System;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public Entity Entity { get; set; }

    private void Awake()
    {
        Init();
    }

    public virtual void Init()
    {
        Entity = GetComponent<Entity>();
    }
}
