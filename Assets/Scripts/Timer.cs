using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Timer : MonoBehaviour
{
    [SerializeField]
    private float maxTime;
    [SerializeField]
    private float currentTime;
    [SerializeField]
    private bool running = true;
    [SerializeField]
    private bool doesReset = true;
    [SerializeField]
    private bool overflows = true;

    public delegate void TimeEvent();

    public event TimeEvent OnComplete;

    public void Initialize(float maxTime, float currentTime = 0)
    {
        this.maxTime = maxTime;
        this.currentTime = currentTime;
    }

    private void Awake()
    {   
        SetTime(currentTime);
        SetMaxTime(maxTime);
    }

    private void OnValidate()
    {
        Awake();
    }

    public virtual bool Tick(float deltaTime, out float time)
    {
        return TickHelper(deltaTime, out time);
    }

    protected bool TickHelper(float deltaTime, out float time)
    {
        if (running)
        {
            currentTime += deltaTime;
            if (currentTime >= maxTime)
            {
                time = currentTime;
                if (doesReset)
                {
                    if (overflows)
                    {
                        currentTime = currentTime - maxTime;
                    }
                    else
                    {
                        currentTime = 0;
                    }
                }
                OnComplete?.Invoke();
                return true;
            }
        }
        time = -1;
        return false;
    }

    public void TurnOff() { running = false; }
    public void TurnOn() { running = true; }
    public void ToggleRunning() { running = !running; }

    public void EnableReset() { doesReset = true; }
    public void DisableReset() { doesReset = false; }
    public void ToggleReset() { doesReset = !doesReset; }

    public bool GetStatus() { return running; }
    public bool GetDoesReset() { return doesReset; }

    public void Restart()
    {
        this.currentTime = 0;
    }

    public void RestartAndHalt()
    {
        Restart();
        TurnOff();
    }

    public void SetMaxTime(float maxTime)
    {
        this.maxTime = maxTime;
    }

    public void SetTime(float time)
    {
        ForceSetTime(time);
        if (currentTime > maxTime)
        {
            Debug.LogWarning("Tried to give timer time after maximum. Use ForceSetTime instead");
            currentTime = maxTime;
        }
    }

    public void ForceSetTime(float time)
    {
        currentTime = time;
    }

    public float GetMaxTime()
    {
        return maxTime;
    }

    public float GetTime()
    {
        return currentTime;
    }

    public float GetTimeRemaining()
    {
        return maxTime - currentTime;
    }

    public float GetPercentLeft()
    {
        return (maxTime - currentTime) / maxTime;
    }

    public float GetPercentPassed()
    {
        return currentTime / maxTime;
    }
}

/*
public class Timer
{
    private float time;
    private float maxTime;

    public Timer(float maxTime, float time=0)
    {
        SetTime(time);
        SetMaxTime(maxTime);
    }
    
    public bool Tick()
    {
        time += TimeController.deltaTime;
        if (time >= maxTime)
        {
            time = time - maxTime;
            return true;
        }
        return false;
    }

    public void SetMaxTime(float maxTime)
    {
        this.maxTime = maxTime;
    }

    public void SetTime(float time)
    {
        this.time = time;
    }

    public float GetMaxTime()
    {
        return maxTime;
    }

    public float GetTime()
    {
        return time;
    }

    public float GetPercentLeft()
    {
        return (maxTime - time) / maxTime;
    }

    public float GetPercentPassed()
    {
        return time / maxTime;
    }
}
*/
