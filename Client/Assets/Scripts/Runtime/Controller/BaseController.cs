using System;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    private Entity _entity;

    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        _entity = GetComponent<Entity>();
    }
}
