using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Stats : MonoBehaviour
{
    [SerializeField] private Stat hp;
    [SerializeField] private Stat attack;
    [SerializeField] private Stat defence;
    [SerializeField] private Stat speed;
    [SerializeField] private Stat actionPoint;

}
