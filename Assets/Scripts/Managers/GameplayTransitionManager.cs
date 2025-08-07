using UnityEngine;
using VContainer;

public class GameplayTransitionManager
{
    [Inject] private readonly EventManager eventManager;
    [Inject] private readonly DeckSelectionManager selectionManager;
    private GameplayConfiguration gameplayConfig;
    private DeckConfiguration config;

    public void Initialize(DeckConfiguration config, GameplayConfiguration gameplayConfig)
    {
        this.config = config;
        this.gameplayConfig = gameplayConfig;
    }

    public void StartGameplay()
    {
        if (!CanStartGameplay())
        {
            Debug.LogWarning("Cannot start gameplay - invalid selection state");
            return;
        }

        TransitionToGameplayUI();
    }

    public void ReturnToDeckSelection()
    {
        TransitionToDeckUI();
    }

    private bool CanStartGameplay()
    {
        if (!selectionManager.IsMaxReached || 
            config.DeckAreaParent == null || 
            config.GameplayAreaParent == null)
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
        if (config.DeckAreaParent != null)
            config.DeckAreaParent.gameObject.SetActive(false);

        if (config.GameplayAreaParent != null)
            config.GameplayAreaParent.gameObject.SetActive(true);

        CreateGameplay3DCards();
    }

    private void TransitionToDeckUI()
    {
        if (config.GameplayAreaParent != null)
            config.GameplayAreaParent.gameObject.SetActive(false);

        if (config.DeckAreaParent != null)
            config.DeckAreaParent.gameObject.SetActive(true);
    }

    private void CreateGameplay3DCards()
    {
        var selectedCards = selectionManager.SelectedCards;
        if (selectedCards.Count == 0) 
        {
            Debug.LogWarning("No selected cards found for gameplay");
            return;
        }

        eventManager?.Publish(new GameplayEvents.CreateGameplay3DCardsRequested(selectedCards));
    }
}
