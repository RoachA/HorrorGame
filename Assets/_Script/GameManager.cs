using System.Globalization;
using Game.UI;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject] private readonly SignalBus _bus;
    [SerializeField] private bool _isDebugging = true;
    
    public float updateInterval = 0.5f; // Update interval in seconds
    private float accum = 0.0f; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval

    private void Start()
    {
        timeleft = updateInterval;
        Cursor.visible = false;
        _bus.Subscribe<CoreSignals.DoorWasOpenedSignal>(OnDoorOpened);
        _bus.Subscribe<CoreSignals.PlayerWasSightedSignal>(OnPlayerSpotted);
    }

    private void OnPlayerSpotted(CoreSignals.PlayerWasSightedSignal signal)
    {
        Debug.Log(signal.Agent.Id + " has spotted player at: " + signal.Time.ToString("F3"));
    }
    
    private void OnDoorOpened(CoreSignals.DoorWasOpenedSignal signal)
    {
        Debug.Log("Door " + signal.Id + " was opened!");
    }

    private void OnDestroy()
    {
        _bus.TryUnsubscribe<CoreSignals.DoorWasOpenedSignal>(OnDoorOpened);
        _bus.TryUnsubscribe<CoreSignals.PlayerWasSightedSignal>(OnPlayerSpotted);
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
