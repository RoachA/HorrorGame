using UnityEngine;
using Zenject;

//https://github.com/modesttree/Zenject/blob/master/Documentation/Signals.md
//https://github.com/modesttree/Zenject/blob/master/Documentation/CheatSheet.md
public class SceneInstaller : MonoInstaller<SceneInstaller>
{
    readonly SignalBus _signalBus;
    
    [SerializeField] private PlayerController _playerController;
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.Bind<PlayerController>().FromInstance(_playerController).AsSingle();
        Container.Bind<SceneInstaller>().AsSingle();
        Container.Bind<CoreSignals>().AsSingle();

        Container.DeclareSignal<CoreSignals.DoorWasOpenedSignal>();
    }
}


