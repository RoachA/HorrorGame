using Zenject;

public class ProjectInstaller : ProjectContext
{
    readonly SignalBus _signalBus;

    public ProjectInstaller(SignalBus signalBus)
    {
        SignalBusInstaller.Install(Container);
        _signalBus = signalBus;
        
        //Project Signals
        Container.DeclareSignal<UserJoinedSignal>();
        Container.BindInterfacesTo<ProjectInstaller>().AsSingle();
    }

    public void Initialize()
    {
    }
}

public class UserJoinedSignal
{
    public string Username;
}

