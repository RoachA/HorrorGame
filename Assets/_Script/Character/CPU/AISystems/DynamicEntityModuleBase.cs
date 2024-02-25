using Game.World;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(LivingEntity))]
public class DynamicEntityModuleBase : MonoBehaviour
{
    [Inject] protected readonly PlayerController _player;
    [Inject] protected readonly SignalBus _bus;

    protected LivingEntity ParentController;
    protected GameObject PlayerRef;

    protected virtual void Awake()
    {
       if (PlayerRef == null)
                PlayerRef = _player.gameObject;

       if (ParentController == null)
           ParentController = GetComponent<LivingEntity>();

       RegisterToParent();
    }

    protected virtual void RegisterToParent()
    {
        ParentController.RegisterModule(this);
    }
}
