using System;
using UnityEngine;

namespace Game.World.Objects
{
   /// <summary>
   /// it is an interactable item.
   /// </summary>
   public interface IInteractable
   {
      protected InteractionMethod InteractionType { get; set; }
      protected bool IsActive { get; set; }
      protected MouseInteractionStats EndStats { get; set; }
      protected MouseInteractionStats StartStats { get; set; }
      public GameObject GetInteractionGameObject();

      public virtual void InteractStart(MouseInteractionStats stats, Action callback = null)
      {
      }

      public virtual void InteractEnd(MouseInteractionStats stats, Action callback = null)
      {
      }
   }
   
   public struct MouseInteractionStats
   {
      public float Time;
      public Vector2 MousePos;

      public MouseInteractionStats(float time, Vector2 mousePos)
      {
         Time = time;
         MousePos = mousePos;
      }
   }

   public enum InteractionMethod
   {
      TapInteraction = 0,
      MouseInteraction = 1,
   }
}
