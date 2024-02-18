using Game.World;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(WorldEntity))]
public class EntityModuleBase : MonoBehaviour
{
    [Inject] protected readonly PlayerController _player;
    [Inject] protected readonly SignalBus _bus;

    protected WorldEntity ParentController;
    protected GameObject PlayerRef;

    protected virtual void Start()
    {
       if (PlayerRef == null)
                PlayerRef = _player.gameObject;

       if (ParentController == null)
           ParentController = GetComponent<WorldEntity>();

       RegisterToParent();
    }

    protected virtual void RegisterToParent()
    {
        ParentController.RegisterModule(this);
    }
}
