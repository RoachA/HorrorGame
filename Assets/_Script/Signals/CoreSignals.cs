using Game.AI;
using Game.World;
using Game.World.Objects;
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

   public class OnLayoutStateUpdateSignal
   {
      public LayoutBase Layout;
      public bool State;

      public OnLayoutStateUpdateSignal(LayoutBase layout, bool state = true)
      {
         Layout = layout;
         State = state;
      }
   }

   public class OnAffectFlashLightSignal
   {
      public FlashLightAction ActionType;
      public float Duration;

      public OnAffectFlashLightSignal(FlashLightAction actionType, float duration = 1)
      {
         ActionType = actionType;
         Duration = duration;
      }
   }

   public class OverwriteMouseLookSensitivitySignal
   {
      public float Sensitivity;

      public OverwriteMouseLookSensitivitySignal(float sensitivity = 1f)
      {
         Sensitivity = sensitivity;
      }
   }

   public class SetCursorSignal
   {
      public bool State;

      public SetCursorSignal(bool state)
      {
         State = state;
      }
   }
}
