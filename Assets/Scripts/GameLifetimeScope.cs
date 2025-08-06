using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [Header("Dependencies")]
    [SerializeField] private CardSO cardSO;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<GameInitializer>();
        
        builder.Register<EventManager>(Lifetime.Singleton);
        builder.Register<AIPlayer>(Lifetime.Singleton);
        builder.Register<ISkillService, SkillService>(Lifetime.Singleton);
        
        builder.Register<CardViewFactory>(Lifetime.Singleton);
        
        // MonoBehaviour injection support
        builder.RegisterComponentInHierarchy<DeckController>();
        builder.RegisterComponentInHierarchy<DropTarget>();
        builder.RegisterComponentInHierarchy<GameplayUI>();
        builder.RegisterComponentInHierarchy<GamePlayController>();
        
        
        builder.RegisterInstance(cardSO);
    }
}
