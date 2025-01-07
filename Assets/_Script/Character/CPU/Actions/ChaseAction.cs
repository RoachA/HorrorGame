using Game.AI;
using UnityEngine;

public class ChaseAction : IAction
{
    public float WaitTime;
    public float ChaseTime;
    public Transform ChaseTarget;

    public ChaseAction(float waitTime, float chaseTime, Transform chaseTarget)
    {
        WaitTime = waitTime;
        ChaseTime = chaseTime;
        ChaseTarget = chaseTarget;
    }
    
    public void Execute()
    {
    }
}
