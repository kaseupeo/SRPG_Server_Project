using UnityEngine;
using UnityEngine.UI;

public class UIHpBar : MonoBehaviour
{
    private Slider _slider;
    
    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }

    public void Change(float current, float max)
    {
        _slider.value = current / max;
    }
}