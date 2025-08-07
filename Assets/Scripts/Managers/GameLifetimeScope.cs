using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private CardSO cardSO;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<GameInitializer>();
        
        // Core Services
        builder.Register<EventManager>(Lifetime.Singleton);
        builder.Register<ICardViewFactory, CardViewFactory>(Lifetime.Singleton);

        //DeckSystem
        builder.Register<IDeckController, DeckController>(Lifetime.Scoped);
        builder.Register<Deck>(Lifetime.Scoped);
        builder.Register<DeckSelectionManager>(Lifetime.Scoped);
        builder.Register<DeckSelectionUIManager>(Lifetime.Scoped);
        builder.Register<CardTransformManager>(Lifetime.Scoped);

        builder.Register<GameplayTransitionManager>(Lifetime.Singleton);

        // Gameplay Systems
        builder.Register<GamePlayController>(Lifetime.Scoped);
        builder.Register<AIPlayer>(Lifetime.Scoped);
        builder.Register<ISkillService, SkillService>(Lifetime.Scoped);
        builder.Register<GameplayStateManager>(Lifetime.Scoped);
        builder.Register<GameplayCardManager>(Lifetime.Scoped);
        builder.Register<GameplayUIController>(Lifetime.Scoped);
        builder.Register<GameplayBattleSystem>(Lifetime.Scoped);
        
        // MonoBehaviour Components
        builder.RegisterComponentInHierarchy<DropTarget>();
        builder.RegisterComponentInHierarchy<GameplayUI>();
        builder.RegisterComponentInHierarchy<GameConfiguration>();
        
        // ScriptableObject Instances
        builder.RegisterInstance(cardSO);
    }
}
