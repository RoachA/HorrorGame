using Game.World.Objects;
using UnityEngine;
using Zenject;

namespace Game.World
{
    public class WorldEntity : MonoBehaviour, IHaveIdentity
    {
        [SerializeField] private string entityName;
        public int Id { get; set; }
        
        public GameObject Object { get; set; }
        
        [Inject] private readonly WorldObjectsContainer _container;

        protected virtual void Start()
        {
            SetupUniqueIdentity();
        }

        private void SetupUniqueIdentity()
        {
            Id = UniqueIDHelper.GenerateUniqueId(this);
            Object = gameObject;
            _container.RegisterObject(this);
            Debug.Log(gameObject.name + " is registered with the unique id: " + Id);
        }
    }
}