using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PowerSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _powerValueText;
    [SerializeField] private int _maxPower = 100;

    public float PowerSliderValue => _slider.value;

    private void OnEnable()
    {
        _slider.onValueChanged.AddListener(OnSliderChange);
        float startSliderValue = 0.5f;
        _slider.value = startSliderValue;
        _powerValueText.text = (startSliderValue * _maxPower).ToString("#.#");

    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(OnSliderChange);
    }

    private void OnSliderChange(float value)
    {
        _powerValueText.text = (value * _maxPower).ToString("#.#");
    }
}
