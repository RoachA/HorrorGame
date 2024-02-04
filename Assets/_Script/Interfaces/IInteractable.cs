using System;
using UnityEngine;

namespace Game.World.Objects
{
   public interface IInteractable
   {
      protected InteractionMethod InteractionType { get; set; }
      protected bool IsActive { get; set; }
      protected InteractionStat EndStat { get; set; }
      protected InteractionStat StartStat { get; set; }
      public GameObject GetInteractionGameObject();
      
      public void InteractStart(InteractionStat stat, Action callback = null);

      public void InteractEnd(InteractionStat stat, Action callback = null);
   }
   
   public struct InteractionStat
   {
      public float Time;
      public Vector2 MousePos;

      public InteractionStat(float time, Vector2 mousePos)
      {
         Time = time;
         MousePos = mousePos;
      }
   }

   public enum InteractionMethod
   {
      Bool = 0,
      ControlledWithX = 1,
      ControllerWithY = 2,
      ControlledWith2D = 3,
   }
}
