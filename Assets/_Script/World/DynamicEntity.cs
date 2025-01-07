using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
    public class DynamicEntity : WorldEntity
    {
        [Header("Modules Registry")]
        [SerializeField] public List<DynamicEntityModuleBase> RegisteredModules = new List<DynamicEntityModuleBase>();

        public void RegisterModule(DynamicEntityModuleBase module)
        {
            RegisteredModules.Add(module);
        }

        public T GetModule<T>() where T : DynamicEntityModuleBase
        {
            foreach (var module in RegisteredModules)
            {
                if (module is T) return module as T;
            }

            return default(T);
        }

        public void SetModuleState<T>(bool state) where T : DynamicEntityModuleBase
        {
            var module = GetModule<T>();
            module.enabled = state;
        }
    }
}