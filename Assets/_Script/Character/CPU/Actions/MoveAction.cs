using System;
using System.Numerics;
using Game.AI;

public class MoveAction : IAction
{
    private Vector3 StartPosition;
    private Vector3 EndPosition;
    private float Speed;
    
    public MoveAction(Vector3 startPos, Vector3 endPos, float spd)
    {
        StartPosition = startPos;
        EndPosition = endPos;
        Speed = spd;
    }
    
    public virtual void Execute()
    {
      
    }
}

