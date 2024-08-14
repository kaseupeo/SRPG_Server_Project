using System;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [field: SerializeField] public GameObject MoveTile { get; set; }
    [field: SerializeField] public GameObject AttackTile { get; set; }
    
    public Vector2Int Position { get; set; }

    private void Start()
    {
        Position = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        MoveTile.SetActive(false);
        AttackTile.SetActive(false);
    }
}
