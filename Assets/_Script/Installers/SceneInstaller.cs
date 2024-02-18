using Game.World;
using UnityEngine;
using Zenject;

//https://github.com/modesttree/Zenject/blob/master/Documentation/Signals.md
//https://github.com/modesttree/Zenject/blob/master/Documentation/CheatSheet.md
public class SceneInstaller : MonoInstaller<SceneInstaller>
{
    readonly SignalBus _signalBus;
    
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private AudioManager _audioManager;
    
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.Bind<PlayerController>().FromInstance(_playerController).AsSingle();
        Container.Bind<AudioManager>().FromInstance(_audioManager).AsSingle();
        Container.Bind<SceneInstaller>().AsSingle();
        Container.Bind<CoreSignals>().AsSingle();
        Container.Bind<TeleportsManager>().AsSingle();
        Container.Bind<UniqueObjectsContainer>().AsSingle();
        
        ///SIGNALS >>>>>>>>>>>>>>>>
        Container.DeclareSignal<CoreSignals.DoorWasOpenedSignal>();
        Container.DeclareSignal<CoreSignals.PlayerWasSightedSignal>();
        Container.DeclareSignal<CoreSignals.PlayerSightWasLostSignal>();
        Container.DeclareSignal<CoreSignals.PlayerTriggeredTeleportZoneSignal>();
        Container.DeclareSignal<CoreSignals.OnTeleportApprovedSignal>();
    }
}


