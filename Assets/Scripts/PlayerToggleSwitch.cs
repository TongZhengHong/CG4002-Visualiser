using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PlayerToggleSwitch : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] private float maxRange = 0.5f;

    [SerializeField, Range(0, 1f)] private float animationDuration = 0.3f;

    [Header("Events")] 
    [SerializeField] private UnityEvent<bool> onPlayerToggle;
    public bool isPlayerOne { get; private set; } = true;

    [SerializeField] private TMP_Text player1Text;
    [SerializeField] private TMP_Text player2Text;
    
    private Slider _slider;

    void Awake()
    {
        SetupSliderComponent();
    }

    private void SetupSliderComponent()
    {
        _slider = GetComponent<Slider>();

        if (_slider == null)
        {
            Debug.Log("No slider found!", this);
            return;
        }

        _slider.interactable = false;
        var sliderColors = _slider.colors;
        sliderColors.disabledColor = Color.white;
        _slider.colors = sliderColors;
        _slider.transition = Selectable.Transition.None;
    }

    void OnEnable()
    {
        if (SettingsController.PlayerNo == 1) 
        {
            _slider.value = 0;
            isPlayerOne = true;

            player1Text.color = Color.white;
            player2Text.color = Color.black;
        } 
        else if (SettingsController.PlayerNo == 2)
        {
            _slider.value = maxRange;
            isPlayerOne = false;

            player1Text.color = Color.black;
            player2Text.color = Color.white;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle(!isPlayerOne);
    }

    private void Toggle(bool state)
    {
        if (state != isPlayerOne)
        {
            onPlayerToggle?.Invoke(state);
        }
        isPlayerOne = state;

        StopAllCoroutines();
        StartCoroutine(AnimateSlider());
    }

    private IEnumerator AnimateSlider()
    {
        float startValue = _slider.value;
        float endValue = isPlayerOne ? 0 : maxRange;

        float time = 0;
        if (animationDuration <= 0) yield break;

        AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);
        while (time < animationDuration)
        {
            time += Time.deltaTime;

            float progress = time / animationDuration;
            float lerpFactor = slideEase.Evaluate(progress);
            _slider.value = Mathf.Lerp(startValue, endValue, lerpFactor);

            if (isPlayerOne) {
                player1Text.color = Color.Lerp(Color.black, Color.white, progress);
                player2Text.color = Color.Lerp(Color.white, Color.black, progress);
            } else { // Player 2
                player1Text.color = Color.Lerp(Color.white, Color.black, progress);
                player2Text.color = Color.Lerp(Color.black, Color.white, progress);
            }

            yield return null;
        }

        _slider.value = endValue;
    }
}
