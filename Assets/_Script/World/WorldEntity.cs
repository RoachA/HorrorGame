using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.World
{
    public class WorldEntity : MonoBehaviour, IHaveIdentity
    {
        [SerializeField] private string entityName;
        public int Id { get; set; }
        
        
        [Header("Modules Registry")]
        [SerializeField] public List<EntityModuleBase> RegisteredModules = new List<EntityModuleBase>();
        
        public GameObject Object { get; set; }
        [Inject] private readonly UniqueObjectsContainer _container;

        protected virtual void Start()
        {
            SetupUniqueIdentity();
        }
        
        public void RegisterModule(EntityModuleBase module)
        {
            RegisteredModules.Add(module);
        }

        public T GetModule<T>() where T : EntityModuleBase
        {
            foreach (var module in RegisteredModules)
            {
                if (module is T) return module as T;
            }

            return default(T);
        }

        public void SetModuleState<T>(bool state) where T : EntityModuleBase
        {
            var module = GetModule<T>();
            module.enabled = state;
        }
        
        void SetupUniqueIdentity()
        {
            Id = UniqueIDHelper.GenerateUniqueId(this);
            Object = gameObject;
            _container.RegisterObject(this);
        }
    }
}