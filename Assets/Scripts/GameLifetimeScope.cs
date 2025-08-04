using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [Header("Dependencies")]
    [SerializeField] private CardSO cardSO;
    
    [Header("Transform References")]
    [SerializeField] private Transform cardParent;
    [SerializeField] private Transform selectedCardsParent;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<GameInitializer>();
        
        builder.Register<EventManager>(Lifetime.Singleton);
        builder.Register<TurnBaseManager>(Lifetime.Singleton);
        
        builder.Register<CardViewFactory>(Lifetime.Singleton);
        
        // MonoBehaviour injection support
        builder.RegisterComponentInHierarchy<DeckController>(); // MonoBehaviour oldu
        
        builder.RegisterInstance(cardSO);
        if (cardParent != null)
            builder.RegisterInstance(cardParent);
    }
}
