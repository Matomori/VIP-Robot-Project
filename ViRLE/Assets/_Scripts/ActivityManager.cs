using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityManager : MonoBehaviour
{
    [SerializeField] protected ActivityData activityData;
    public event Action<ActivityData> OnStartActivity;

    public void StartActivity() {
        OnStartActivity?.Invoke(activityData);
    }

    /// <summary>
    /// Essentially a dialogue interupt / letting extra conditions that need to spawn with certain dialogue pages
    /// </summary>
    public virtual void CheckConditions() { }
}
