using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TimeSettings timeSettings;

    [SerializeField] Light sun;
    [SerializeField] Light moon;
    [SerializeField] AnimationCurve lightIntensityCurve;
    [SerializeField] float maxSunIntensity = 1;
    [SerializeField] float maxMoonIntensity = 0.5f;

    /// <summary>
    /// This is for URP
    /// </summary>
    //[SerializeField] Color dayAmbientLight;
    //[SerializeField] Color nightAmbientLight;
    //[SerializeField] Volume volume;
    //ColorAdjustments colorAdjustments;
    [SerializeField] Material skyboxMaterial;

    [SerializeField] RectTransform dial;
    float initialDialRotation;

    public event Action OnSunrise
    {
        add => timeService.OnSunrise += value;
        remove => timeService.OnSunrise -= value;
    }

    public event Action OnSunset
    {
        add => timeService.OnSunset += value;
        remove => timeService.OnSunset -= value;
    }

    public event Action OnHourChange
    {
        add => timeService.OnHourChange += value;
        remove => timeService.OnHourChange -= value;
    }

    TimeService timeService;

    private void Start()
    {
        timeService = new TimeService(timeSettings);
        //volume.profile.TryGet(out colorAdjustments);

        OnSunrise += () => Debug.Log("Sunrise");
        OnSunset += () => Debug.Log("Sunset");
        OnHourChange += () => Debug.Log("Hour Change");

        initialDialRotation = dial.rotation.eulerAngles.z;
    }

    private void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLightSettings();
        UpdateSkyBlend();
    }

    void UpdateSkyBlend()
    {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.up);
        float blend = Mathf.Lerp(0, 1, lightIntensityCurve.Evaluate(dotProduct));
        skyboxMaterial.SetFloat("_Blend", blend);
    }

    void UpdateLightSettings()
    {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);
        sun.intensity = Mathf.Lerp(0 ,maxSunIntensity, lightIntensityCurve.Evaluate(dotProduct));
        moon.intensity = Mathf.Lerp(0, maxMoonIntensity, lightIntensityCurve.Evaluate(-dotProduct));

        //if (colorAdjustments == null) return;
        //colorAdjustments.colorFilter.value = Color.Lerp(nightAmbientLight, dayAmbientLight, lightIntensityCurve.Evaluate(dotProduct));
    }

    void RotateSun()
    {
        float rotation = timeService.CalculateSunAngle();
        sun.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.right);

        dial.rotation = Quaternion.Euler(0, 0, initialDialRotation + rotation);
    }

    void UpdateTimeOfDay()
    {
        timeService.UpdateTime(Time.deltaTime);
        if (timeText != null)
        {
            timeText.text = timeService.CurrentTime.ToString("HH:mm");
        }
    }
}
