using UnityEngine;

namespace Game.World.Objects
{
    public class KeyEntity : ObtainableEntity, IUnlock
    {
        private InteractionMethod m_interactionType;
        
        protected override void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position + Vector3.up * 1.5f, "key_gizmo");
        }
    }
}