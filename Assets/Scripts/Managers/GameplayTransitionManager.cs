using System.Threading.Tasks;
using UnityEngine;

public class GameplayTransitionManager
{
    private readonly DeckConfiguration config;
    private readonly SelectionManager selectionManager;
    private readonly CardViewFactory cardViewFactory;
    private readonly GameplayConfiguration gameplayConfig;

    public GameplayTransitionManager(
        DeckConfiguration config,
        GameplayConfiguration gameplayConfig,
        SelectionManager selectionManager,
        CardViewFactory cardViewFactory)
    {
        this.config = config;
        this.gameplayConfig = gameplayConfig;
        this.selectionManager = selectionManager;
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

    private async Task CreateGameplay3DCards()
    {
        var selectedCards = selectionManager.SelectedCards;
        if (selectedCards.Count == 0) return;

        var card3DViews = await cardViewFactory.CreateCard3DViewsAtPositionsAsync(
            selectedCards, 
            gameplayConfig
        );

        Debug.Log($"Created {card3DViews.Count} 3D cards at designated positions");
    }
}
