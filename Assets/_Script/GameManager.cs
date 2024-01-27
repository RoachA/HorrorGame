using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Game.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool _isDebugging = true;
    
    public float updateInterval = 0.5f; // Update interval in seconds
    private float accum = 0.0f; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval

    private void Start()
    {
        timeleft = updateInterval;
    }

    void Update()
    {
        if (_isDebugging)
            OnDebugActive();
    }

    private void OnDebugActive()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;
        
        if (timeleft <= 0.0f)
        {
            float fps = accum / frames;
            ScreenDubegger.Fps = fps.ToString(CultureInfo.InvariantCulture);
            // Reset variables for the next interval
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }
}
