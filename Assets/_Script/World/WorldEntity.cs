using UnityEngine;
using Zenject;

namespace Game.World
{
    public class WorldEntity : MonoBehaviour, IHaveIdentity
    {
        [SerializeField] private string entityName;
        public int Id { get; set; }
        public GameObject Object { get; set; }
        [Inject] private readonly UniqueObjectsContainer _container;

        protected virtual void Start()
        {
            SetupUniqueIdentity();
        }
        
        void SetupUniqueIdentity()
        {
            Id = UniqueIDHelper.GenerateUniqueId(this);
            Object = gameObject;
            _container.RegisterObject(this);
        }
    }
}