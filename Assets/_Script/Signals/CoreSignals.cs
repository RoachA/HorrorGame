using System.Collections;
using System.Collections.Generic;
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
}
