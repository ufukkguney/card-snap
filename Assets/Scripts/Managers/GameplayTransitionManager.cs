using System.Threading.Tasks;
using UnityEngine;

public class GameplayTransitionManager
{
    private readonly DeckConfiguration config;
    private readonly SelectionManager selectionManager;
    private readonly GameplayConfiguration gameplayConfig;
    private readonly EventManager eventManager;

    public GameplayTransitionManager(
        DeckConfiguration config,
        GameplayConfiguration gameplayConfig,
        SelectionManager selectionManager,
        CardViewFactory cardViewFactory,
        EventManager eventManager)
    {
        this.config = config;
        this.gameplayConfig = gameplayConfig;
        this.selectionManager = selectionManager;
        this.eventManager = eventManager;
        // Note: CardViewFactory kept in constructor for backward compatibility
        // but not stored as it's now handled by GamePlayController via events
    }

    public async void StartGameplay()
    {
        if (!CanStartGameplay())
        {
            Debug.LogWarning("Cannot start gameplay - invalid selection state");
            return;
        }

        TransitionToGameplayUI();
        CreateGameplay3DCards();
        
        Debug.Log("Gameplay transition completed successfully");
    }

    public void ReturnToDeckSelection()
    {
        TransitionToDeckUI();
        Debug.Log("Returned to deck selection");
    }

    private bool CanStartGameplay()
    {
        if (!selectionManager.IsMaxReached || 
            config.deckAreaParent == null || 
            config.gameplayAreaParent == null)
        {
            return false;
        }

        if (gameplayConfig != null && !gameplayConfig.ArePositionsValid())
        {
            Debug.LogWarning("GameplayConfiguration has invalid positions - will use fallback method");
        }

        return true;
    }

    private void TransitionToGameplayUI()
    {
        if (config.deckAreaParent != null)
            config.deckAreaParent.gameObject.SetActive(false);
        
        if (config.gameplayAreaParent != null)
            config.gameplayAreaParent.gameObject.SetActive(true);
        
        Debug.Log("UI transitioned to gameplay mode");
    }

    private void TransitionToDeckUI()
    {
        if (config.gameplayAreaParent != null)
            config.gameplayAreaParent.gameObject.SetActive(false);
        
        if (config.deckAreaParent != null)
            config.deckAreaParent.gameObject.SetActive(true);
        
        Debug.Log("UI transitioned to deck selection mode");
    }

    private void CreateGameplay3DCards()
    {
        var selectedCards = selectionManager.SelectedCards;
        if (selectedCards.Count == 0) 
        {
            Debug.LogWarning("No selected cards found for gameplay");
            return;
        }

        // Request card creation via event system
        eventManager?.Publish(new GameplayEvents.CreateGameplay3DCardsRequested(selectedCards));
        
        // Small delay to allow event processing
        // await Task.Delay(100);
    }
}
