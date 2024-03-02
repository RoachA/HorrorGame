using UnityEngine;

namespace Game.UI
{
   public interface IUiPanelBase
   {
      public void Open();

      public void Close();
   }

   public abstract class UiPanel : MonoBehaviour, IUiPanelBase
   {
      public virtual void Open()
      {
         Init();
         gameObject.SetActive(true);
      }

      public virtual void Close()
      {
         gameObject.SetActive(false);
      }
      
      protected abstract void Init();
   }
}