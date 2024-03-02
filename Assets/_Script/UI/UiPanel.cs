using UnityEngine;
using Zenject;

namespace Game.UI
{
   public interface IUiPanelBase
   {
      public void Open(UIPanelParams data);

      public void Close();
   }

   public interface IUiParams
   {
   }

   public class UIPanelParams : IUiParams
   {
   }

   public abstract class UiPanel : MonoBehaviour, IUiPanelBase, IUiParams
   {
      [Inject] protected UIManager _uiManager;
      
      public void Open(UIPanelParams data)
      {
         Init(data);
         gameObject.SetActive(true);
      }

      public virtual void Close()
      {
         gameObject.SetActive(false);
      }

      protected abstract void Init(UIPanelParams data);
   }
}