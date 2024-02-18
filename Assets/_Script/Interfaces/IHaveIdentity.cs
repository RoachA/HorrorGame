using UnityEngine;
using Zenject;

namespace Game.World
{
    public interface IHaveIdentity
    {
        /// <summary>
        /// Also when generated, you may want to register this to UniqueObjectsContainer
        /// this container was registered as a single dependency on scene installer.
        /// </summary>
        /// 
        int Id { get; set; }
        GameObject Object { get; set; }
    }
}