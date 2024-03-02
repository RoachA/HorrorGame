using UnityEngine;
namespace Game.World.Objects
{
    /// <summary>
    /// a unique and identifiable item.
    /// </summary>
    /// 
    public interface IHaveIdentity
    {
        /// <summary>
        /// Also when generated, you may want to register this to WorldObjectsContainer
        /// this container was registered as a single dependency on scene installer.
        /// </summary>
        /// 
        int Id { get; set; }
        GameObject Object { get; set; }
    }
}