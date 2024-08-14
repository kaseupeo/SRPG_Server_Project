using UnityEngine;

[CreateAssetMenu(fileName = "NewStat", menuName = "Stat", order = 0)]
public class Stat : ScriptableObject
{
    public delegate void ValueChangedHandler(Stat stat, float currentValue, float prevValue);
    
    [field: SerializeField] private float MaxValue { get; set; }
    [field: SerializeField] private float MinValue { get; set; }
    [SerializeField] private float defaultValue;

    public float DefaultValue
    {
        get => defaultValue;
        set
        {
            float prevValue = value;
            defaultValue = Mathf.Clamp(value, MinValue, MaxValue);
        }
    }
}