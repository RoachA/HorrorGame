using Game.Player;
using Game.UI;
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
    [SerializeField] private GameplayPanelUi _gameplayUI;
    
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.Bind<SceneInstaller>().AsSingle();
        Container.Bind<PlayerController>().FromInstance(_playerController).AsSingle();
        Container.Bind<AudioManager>().FromInstance(_audioManager).AsSingle();
        Container.Bind<GameplayPanelUi>().FromInstance(_gameplayUI).AsSingle();
        
        Container.Bind<CoreSignals>().AsSingle();
        Container.Bind<TeleportsManager>().AsSingle();
        Container.Bind<WorldObjectsContainer>().AsSingle();
        Container.Bind<PlayerInventoryManager>().AsSingle();

        ///SIGNALS >>>>>>>>>>>>>>>>
        Container.DeclareSignal<CoreSignals.DoorWasOpenedSignal>();
        Container.DeclareSignal<CoreSignals.PlayerWasSightedSignal>();
        Container.DeclareSignal<CoreSignals.PlayerSightWasLostSignal>();
        Container.DeclareSignal<CoreSignals.PlayerTriggeredTeleportZoneSignal>();
        Container.DeclareSignal<CoreSignals.OnTeleportApprovedSignal>();
        Container.DeclareSignal<CoreSignals.OnLayoutStateUpdateSignal>();
        Container.DeclareSignal<CoreSignals.OnAffectFlashLightSignal>();
    }
}


