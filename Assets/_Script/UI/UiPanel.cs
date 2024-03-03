using System.Runtime.InteropServices;
using UnityEngine;
using Zenject;

namespace Game.UI
{
   public interface IUiPanelBase
   {
      public void Open(UIPanelParams data, bool handleMouse = true);

      public void Close(bool handleMouse = true);
   }

   public interface IUiParams
   {
   }

   public class UIPanelParams : IUiParams
   {
   }

   public abstract class UiPanel : MonoBehaviour, IUiPanelBase, IUiParams
   {
      [Inject] protected readonly UIManager _uiManager;
      [Inject] protected readonly SignalBus _bus;
      
      public void Open(UIPanelParams data, bool handleMouse = true)
      {
         Init(data);
         gameObject.SetActive(true);
         
         if (handleMouse == false) return;
         
         _bus.Fire(new CoreSignals.OverwriteMouseLookSensitivitySignal(0.08f));
         _bus.Fire(new CoreSignals.SetCursorSignal(true));
      }

      public virtual void Close(bool handleMouse = true)
      {
         gameObject.SetActive(false);
         
         if (handleMouse == false) return;
         
         _bus.Fire(new CoreSignals.OverwriteMouseLookSensitivitySignal(1f));
         _bus.Fire(new CoreSignals.SetCursorSignal(false));
      }

      protected abstract void Init(UIPanelParams data);
   }
}