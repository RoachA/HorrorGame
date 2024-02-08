using Zenject;

//https://github.com/modesttree/Zenject/blob/master/Documentation/Signals.md
//https://github.com/modesttree/Zenject/blob/master/Documentation/CheatSheet.md
public class SceneInstaller : MonoInstaller<SceneInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerController>().FromComponentInHierarchy(true).AsSingle();
        Container.Bind<SceneInstaller>().AsSingle();
    }
}


