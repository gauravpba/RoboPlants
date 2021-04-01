using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerUtility : MonoBehaviour
{
    #region Variables

    [Header("Events")]
    [HideInInspector] public UnityEvent OnTimeExpired;

    IEnumerator timerCoroutine;
    bool timerInProgress = false;
    public delegate bool TimerCondition(); //Delegate used as parameter type with conditional timer

    float count = 0f; //Timer time tracker

    #endregion

    #region Unity Methods

    private void Awake()
    {
        OnTimeExpired = new UnityEvent();
    }

    #endregion

    #region Custom Methods

    //Start the timer
    public void StartTimer(float timerAmount)
    {
        Debug.Log("Start Timer");

        timerCoroutine = TimerCoroutine(timerAmount);
        StartCoroutine(timerCoroutine);
    }

    public void StartConditionalTimer(TimerCondition timerCondition)
    {
        timerCoroutine = ConditionalTimerCoroutine(timerCondition);
        StartCoroutine(timerCoroutine);
    }

    public void StartIntervalConditionalTimer(float timerAmount, TimerCondition timerCondition)
    {
        timerCoroutine = IntervalConditionalTimerCoroutine(timerAmount, timerCondition);
        StartCoroutine(timerCoroutine);
    }

    //Used to prematurely end the timer
    public void StopTimer()
    {
        Debug.Log("Stop Timer");

        //Stop the timer coroutine
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerInProgress = false;
        }

        //Destroy the TimerUtility component
        Destroy(this);
    }

    public bool TimerInProgress()
    {
        return timerInProgress;
    }

    //Timer coroutine
    IEnumerator TimerCoroutine(float timerAmount)
    {
        timerInProgress = true;
        count = 0f;

        while (count < timerAmount)
        {
            count += Time.deltaTime;
            yield return null;
        }

        //Remove reference to coroutine
        timerInProgress = false;
        timerCoroutine = null;

        //Invoke the OnTimerFinished event
        OnTimeExpired.Invoke();

        //Destroy the TimerUtility component
        Destroy(this);
    }

    //Timer coroutine
    IEnumerator ConditionalTimerCoroutine(TimerCondition timerCondition)
    {
        timerInProgress = true;

        //While the result of timercondition is false, repeat
        while (!timerCondition()) yield return new WaitForSeconds(0.1f);

        //Remove reference to coroutine
        timerInProgress = false;
        timerCoroutine = null;

        //Invoke the OnTimerFinished event
        OnTimeExpired.Invoke();

        //Destroy the TimerUtility component
        Destroy(this);
    }

    //Invokes OnTimeExpired every timerAmount seconds, and stops if timerCondition is met.
    IEnumerator IntervalConditionalTimerCoroutine(float timerAmount, TimerCondition timerCondition)
    {
        timerInProgress = true;

        while (!timerCondition())
        {
            count = 0f;
            while (count < timerAmount)
            {
                count += Time.deltaTime;
                yield return null;
            }
        }

        //Invoke the OnTimerFinished event
        OnTimeExpired.Invoke();

        //Remove reference to coroutine
        timerInProgress = false;
        timerCoroutine = null;

        //Destroy the TimerUtility component
        Destroy(this);
    }

    #endregion
}
