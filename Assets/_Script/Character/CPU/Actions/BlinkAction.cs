using Game.AI;

public class BlinkAction : IAction
{
    public float WaitTime;
    public BlinkStartType StartType;

    public BlinkAction(float waitTime, BlinkStartType startType)
    {
        WaitTime = waitTime;
        StartType = startType;
    }
    
    public virtual void Execute()
    {
    }
}

public enum BlinkStartType
{
    OnPlayerView = 0,
}
