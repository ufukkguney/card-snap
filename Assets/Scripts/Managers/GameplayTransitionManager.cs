using System.Collections.Generic;
using UnityEngine;

public class GameplayTransitionManager
{
    private readonly DeckConfiguration config;
    private readonly SelectionManager selectionManager;
    private readonly EventManager eventManager;
    private readonly CardViewFactory cardViewFactory;

    public GameplayTransitionManager(
        DeckConfiguration config, 
        SelectionManager selectionManager, 
        EventManager eventManager,
        CardViewFactory cardViewFactory)
    {
        this.config = config;
        this.selectionManager = selectionManager;
        this.eventManager = eventManager;
        this.cardViewFactory = cardViewFactory;
    }

    public async void StartGameplay()
    {
        if (!CanStartGameplay())
        {
            Debug.LogWarning("Cannot start gameplay - invalid selection state");
            return;
        }

        try
        {
            TransitionToGameplayUI();
            
            await CreateGameplay3DCards();
            
            Debug.Log("Gameplay transition completed successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Gameplay transition failed: {e.Message}");
            TransitionToDeckUI();
        }
    }

    public void ReturnToDeckSelection()
    {
        TransitionToDeckUI();
        Debug.Log("Returned to deck selection");
    }

    private bool CanStartGameplay()
    {
        // Temel kontroller
        if (!selectionManager.IsMaxReached || 
            config.deckAreaParent == null || 
            config.gameplayAreaParent == null)
        {
            return false;
        }

        // GameplayConfiguration kontrolü
        if (config.gameplayConfig != null && !config.gameplayConfig.ArePositionsValid())
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

    private async System.Threading.Tasks.Task CreateGameplay3DCards()
    {
        var selectedCards = selectionManager.SelectedCards;
        if (selectedCards.Count == 0) return;

        // GameplayConfiguration kontrolü
        if (config.gameplayConfig == null || !config.gameplayConfig.ArePositionsValid())
        {
            Debug.LogError("Invalid gameplay configuration - falling back to parent-based creation");
            
            // Fallback: Normal parent-based creation
            var fallbackCards = await cardViewFactory.CreateCard3DViewsAsync(
                selectedCards, 
                config.gameplayAreaParent
            );
            Debug.Log($"Created {fallbackCards.Count} 3D cards using fallback method");
            return;
        }

        // Position-based creation ile gelişmiş instantiation
        var card3DViews = await cardViewFactory.CreateCard3DViewsAtPositionsAsync(
            selectedCards, 
            config.gameplayConfig
        );

        Debug.Log($"Created {card3DViews.Count} 3D cards at designated positions");
    }
}
