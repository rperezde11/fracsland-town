using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class TimeOfDayExtensionMethods
{
    public static bool IsTransition(this DayLight.TimeOfDay time)
    {
        int e = (int)time;

        return (e & 1) == 1;
    }

    public static bool IsDayTime(this DayLight.TimeOfDay time)
    {
        int e = (int)time;

        return (e & 2) == 2;
    }

}

[ExecuteInEditMode]
public class DayLight : MonoBehaviour 
{
    public TimeOfDay timeOfDay { get; private set; }
    
    private const int MINUTES_BY_HOUR = 60;
    private const int HOURS_BY_DAY = 24;

    private float angle = 0;
    private float timer = 0;
    private float minutes = 0;
    private Light directionalLight;

    public float speed = 10;
    public int hour = 0;
    public int minute = 0;

    public Color dayTimeMin;
    public Color dayTimeMax;
    public Color nightTimeMin;
    public Color nightTimeMax;

    public enum TimeOfDay
    {
        MORNING   = 3,
        AFTERNOON = 2,
        EVENING   = 1,
        NIGHT     = 0
    }

    private Dictionary<TimeOfDay, int> Schedule = new Dictionary<TimeOfDay, int>
    {
        { TimeOfDay.MORNING,   6  },
        { TimeOfDay.AFTERNOON, 7  },
        { TimeOfDay.EVENING,   18 },
        { TimeOfDay.NIGHT,     19 }
    };

    // Use this for initialization
    void Start ()
    {
        directionalLight = GetComponent<Light>();

        if (!directionalLight) 
        {
            directionalLight = gameObject.AddComponent<Light>();
            directionalLight.type = LightType.Directional;
        }

        if (directionalLight.type != LightType.Directional)
        {
            throw new System.Exception("Light Must be of type directional");
        }

        SetLightByTime(hour, minute);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Application.isPlaying)
        {
            timer += Time.deltaTime * speed;

            if (timer > 1)
            {
                minute = (minute + 1) % MINUTES_BY_HOUR;
                minutes++;

                if (minute == 0)
                {
                    hour = (hour + 1) % HOURS_BY_DAY;
                }

                timer = 0;

                UpdateTimeOfDay();
            }
        }

        SetLightByTime(hour, minute + timer);
    }

    private void SetLightByTime(int hour, float minute)
    {
        timeOfDay = GetTimeOfDay(hour);
        minutes   = ( ((timeOfDay == TimeOfDay.NIGHT && hour < 18) ? 24 : 0) + hour - Schedule[timeOfDay]) * MINUTES_BY_HOUR + minute;
        angle     = CalculateLightAngleByTime(hour, minute);
        
        // Set colors
        float weight =  (timeOfDay.IsTransition() ? 1 : 11) * MINUTES_BY_HOUR;
        weight = minutes / weight;

        if (timeOfDay == TimeOfDay.MORNING) {
            directionalLight.color = Color.Lerp(nightTimeMin, dayTimeMin, weight);
        } else if (timeOfDay == TimeOfDay.AFTERNOON) {
            directionalLight.color = Color.Lerp(dayTimeMin, dayTimeMax, -Mathf.Abs(2*weight-1)+1);
        } else if (timeOfDay == TimeOfDay.EVENING) {
            directionalLight.color = Color.Lerp(dayTimeMin, nightTimeMin, weight);
        } else {
            directionalLight.color = Color.Lerp(nightTimeMin, nightTimeMax, -Mathf.Abs(2*weight-1)+1);
        }

        directionalLight.transform.localRotation = Quaternion.Euler(angle, 0, 0);

        if (Application.isPlaying)
        {
            directionalLight.intensity = CalculateLightIntensityByTime(hour, minute);
        }
    }

    private float CalculateLightIntensityByTime(float hour, float minute)
    {
        float time = hour  + minute/60f;

        return -Mathf.Abs((1f/12f) * (time-12f)) + 1.2f;
    }

    private float CalculateLightAngleByTime(float hour, float minute)
    {
        float m, hours;

        TimeOfDay timerange = GetTimeOfDay(hour);

        m = 90f * (timeOfDay.IsTransition() ? -1f : 1/11f); 

        hours = minutes /  MINUTES_BY_HOUR;

        return m * hours + ( timeOfDay.IsTransition() ? 135f : 45f );
    }

    // Time Of Day

    private TimeOfDay GetTimeOfDay(float h)
    {
        int isTransition = System.Convert.ToInt16(h == 6 || h == 18);
        int isDayTime = System.Convert.ToInt16(h < 18 && h >= 6) << 1;

        return (TimeOfDay)(isTransition + isDayTime);
    }

    private void UpdateTimeOfDay()
    {
        TimeOfDay temp = GetTimeOfDay(hour);

        if (temp != timeOfDay)
        {
            minutes = 0;
            timeOfDay = temp;
        }
    }

}
