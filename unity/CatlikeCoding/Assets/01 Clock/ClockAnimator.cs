using System;

using UnityEngine;

public class ClockAnimator : MonoBehaviour {
    public Transform hours, minutes, seconds;

    public bool analog;

    private const float 
        HOURS_TO_DEGREES = 360f / 12f, 
        MINUTES_TO_DEGREES = 360f / 60f, 
        SECONDS_TO_DEGREES = 360f / 60f;

    private void Update() {
        if(analog) {
            TimeSpan timespan = DateTime.Now.TimeOfDay;

            hours.localRotation = Quaternion.Euler(
                0f, 0f, (float)timespan.TotalHours * -HOURS_TO_DEGREES);
            minutes.localRotation = Quaternion.Euler(
                0f, 0f, (float)timespan.TotalMinutes * -MINUTES_TO_DEGREES);
            seconds.localRotation = Quaternion.Euler(
                0f, 0f, (float)timespan.TotalSeconds * -SECONDS_TO_DEGREES);
        } else {
            DateTime time = DateTime.Now;

            hours.localRotation =
                Quaternion.Euler(0f, 0f, time.Hour * -HOURS_TO_DEGREES);
            minutes.localRotation =
                Quaternion.Euler(0f, 0f, time.Minute * -MINUTES_TO_DEGREES);
            seconds.localRotation =
                Quaternion.Euler(0f, 0f, time.Second * -SECONDS_TO_DEGREES);
        }
    }
}