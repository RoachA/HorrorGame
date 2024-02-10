using System.Collections;
using System.Collections.Generic;
using Game.World;
using UnityEngine;

public class CoreSignals : MonoBehaviour
{
   public class DoorWasOpenedSignal
   {
      public int Id;

      public DoorWasOpenedSignal(int id)
      {
         Id = id;
      }
   }

   public class PlayerWasSightedSignal
   {
      public IHaveIdentity Agent;
      public float Time;
      
      public PlayerWasSightedSignal(IHaveIdentity agent, float time)
      {
         Agent = agent;
         Time = time;
      }
   }

   public class PlayerSightWasLostSignal
   {
      public IHaveIdentity Agent;
      public float Time;
      
      public PlayerSightWasLostSignal(IHaveIdentity agent, float time)
      {
         Agent = agent;
         Time = time;
      }
   }
}
