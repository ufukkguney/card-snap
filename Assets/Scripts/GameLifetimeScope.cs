using System.Threading;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<GameInitializer>();
        builder.Register<TurnBaseManager>(Lifetime.Singleton);
    }
}
