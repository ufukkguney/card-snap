using VContainer;
using System;

public class GamePlayController : IDisposable
{
    [Inject] private EventManager eventManager;
    [Inject] private GameplayBattleSystem battleSystem;
    [Inject] private GameplayStateManager stateManager;
    [Inject] private GameplayCardManager cardManager;
    [Inject] private GameplayUIController uiController;
    [Inject] private GameConfiguration gameConfig;

    private GameplayConfiguration gameplayConfig => gameConfig?.GameplayConfiguration;

    [Inject] 
    public void Construct()
    {
        SubscribeToEvents();
        stateManager.Initialize();
        battleSystem.Initialize(gameplayConfig);
        cardManager.Initialize(gameplayConfig);
        uiController.UpdateHealthDisplay(stateManager.PlayerHealth, stateManager.AIHealth);
    }

    private void SubscribeToEvents()
    {
        eventManager.Subscribe<GameplayEvents.UseSkillRequested>(evt => battleSystem.UseSkill());
        eventManager.Subscribe<GameplayEvents.EndTurnRequested>(evt => battleSystem.EndTurn());
        eventManager.Subscribe<GameplayEvents.RetryGameRequested>(evt => stateManager.ResetGame());
        eventManager.Subscribe<GameplayEvents.CreateGameplay3DCardsRequested>(evt => _ = cardManager.CreateCards(evt.SelectedCards));
    }

    public void OnCardPlacedOnTarget(DropTarget target, Card3DView cardView) => 
        cardManager.SetCurrentPlayerCard(cardView);
    
    public void OnCardRemovedFromTarget(DropTarget target, Card3DView cardView) => 
        cardManager.SetCurrentPlayerCard(null);

    public void Dispose()
    {
        eventManager?.Unsubscribe<GameplayEvents.UseSkillRequested>(evt => battleSystem.UseSkill());
        eventManager?.Unsubscribe<GameplayEvents.EndTurnRequested>(evt => battleSystem.EndTurn());
        eventManager?.Unsubscribe<GameplayEvents.RetryGameRequested>(evt => stateManager.ResetGame());
        eventManager?.Unsubscribe<GameplayEvents.CreateGameplay3DCardsRequested>(evt => _ =cardManager.CreateCards(evt.SelectedCards));
    }
}
