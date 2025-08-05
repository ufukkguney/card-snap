using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using VContainer;
using System.Threading.Tasks;

public class GamePlayController : MonoBehaviour, IDisposable
{
    public GameplayConfiguration gameplayConfig;
    
    private Card3DView currentPlayerCard;
    private EventManager eventManager;
    private AIPlayer aiPlayer;
    private GameplayUI gameplayUI;
    private CardViewFactory cardViewFactory;
    private int playerHealth = 200;
    private int aiPlayerHealth = 200;
    private int currentTurn = 0;
    private const int MAX_TURNS = 6;
    
    [Inject]
    public void Construct(EventManager eventManager, AIPlayer aiPlayer, GameplayUI gameplayUI, CardViewFactory cardViewFactory)
    {
        (this.eventManager, this.aiPlayer, this.gameplayUI, this.cardViewFactory) = (eventManager, aiPlayer, gameplayUI, cardViewFactory);
        SubscribeToEvents();
        UpdateHealthDisplay();
    }
    
    public void OnCardPlacedOnTarget(DropTarget target, Card3DView cardView) => 
        currentPlayerCard = cardView;
    
    public void OnCardRemovedFromTarget(DropTarget target, Card3DView cardView) => 
        currentPlayerCard = null;
    
    private void SubscribeToEvents()
    {
        eventManager.Subscribe<GameplayEvents.UseSkillRequested>(OnUseSkillRequested);
        eventManager.Subscribe<GameplayEvents.EndTurnRequested>(OnEndTurnRequested);
        eventManager.Subscribe<GameplayEvents.RetryGameRequested>(OnRetryGameRequested);
        eventManager.Subscribe<GameplayEvents.CreateGameplay3DCardsRequested>(OnCreateGameplay3DCardsRequested);
    }
    
    private void OnUseSkillRequested(GameplayEvents.UseSkillRequested evt) =>
        Debug.Log("Use Skill Requested");
    
    private void OnEndTurnRequested(GameplayEvents.EndTurnRequested evt) =>
        _ = CreateAICardAsync();
    
    private void OnRetryGameRequested(GameplayEvents.RetryGameRequested evt)
    {
        Debug.Log("Retry game requested - initiating complete reset");
        PerformCompleteGameReset();
    }
    
    private async void OnCreateGameplay3DCardsRequested(GameplayEvents.CreateGameplay3DCardsRequested evt)
    {
        Debug.Log("3D card creation requested via event");
        await CreateGameplay3DCards(evt.SelectedCards);
    }
    
    private async Task CreateAICardAsync()
    {
        if (aiPlayer == null || gameplayConfig?.AiDropTarget == null || currentTurn >= MAX_TURNS) return;
       
        currentTurn++;
        var aiCard = await aiPlayer.CreateRandomCardViewAsync(gameplayConfig.AiDropTarget.transform);
        if (aiCard != null)
        {
            gameplayConfig.AiDropTarget.PlaceCard(aiCard);
            ProcessBattle(currentPlayerCard, aiCard);
            await Task.Delay(3000);
            DestroyBattleCards(currentPlayerCard, aiCard);
        }
    }
    
    private void ProcessBattle(Card3DView playerCard, Card3DView aiCard)
    {
        if (playerCard == null || aiCard == null) return;
        
        var playerDamage = Mathf.Max(0, playerCard.Data.Attack - aiCard.Data.Defense);
        var aiDamage = Mathf.Max(0, aiCard.Data.Attack - playerCard.Data.Defense);
        
        aiPlayerHealth -= playerDamage;
        playerHealth -= aiDamage;
        
        UpdateHealthDisplay();
        LogBattleResult(playerDamage, aiDamage);
        CheckGameOver();
    }
    
    private void UpdateHealthDisplay() => gameplayUI?.UpdateHealthDisplay(playerHealth, aiPlayerHealth);
    
    private void LogBattleResult(int playerDamage, int aiDamage) =>
        Debug.Log($"Turn {currentTurn}/{MAX_TURNS} - Battle: Player dealt {playerDamage} | AI dealt {aiDamage} | Health: Player {playerHealth} | AI {aiPlayerHealth}");
    
    private void CheckGameOver()
    {
        var sb = new StringBuilder();
        
        if (playerHealth <= 0)
            sb.Append("AI WINS!").AppendLine().Append("Player health reached 0");
        else if (aiPlayerHealth <= 0)
            sb.Append("PLAYER WINS!").AppendLine().Append("AI health reached 0");
        else if (currentTurn >= MAX_TURNS)
        {
            if (playerHealth > aiPlayerHealth)
                sb.Append("PLAYER WINS!").AppendLine().Append($"{playerHealth} vs {aiPlayerHealth} health after {MAX_TURNS} turns");
            else if (aiPlayerHealth > playerHealth)
                sb.Append("AI WINS!").AppendLine().Append($"{aiPlayerHealth} vs {playerHealth} health after {MAX_TURNS} turns");
            else
                sb.Append("DRAW!").AppendLine().Append($"Both have {playerHealth} health after {MAX_TURNS} turns");
        }
        
        if (sb.Length > 0) gameplayUI?.ShowGameFinishedPanel(sb.ToString());
    }
    
    private void DestroyBattleCards(Card3DView playerCard, Card3DView aiCard)
    {
        cardViewFactory?.DestroyCardView(playerCard);
        cardViewFactory?.DestroyCardView(aiCard);
        currentPlayerCard = null;
    }
    
    /// <summary>
    /// Resets all gameplay state to initial values
    /// </summary>
    public void ResetGameState()
    {
        // Reset health values
        playerHealth = 200;
        aiPlayerHealth = 200;
        
        // Reset turn counter
        currentTurn = 0;
        
        // Clear current player card
        currentPlayerCard = null;
        
        // Hide game finished panel
        gameplayUI?.HideGameFinishedPanel();
        
        // Update health display
        UpdateHealthDisplay();
        
        Debug.Log("GamePlay state reset to initial values");
    }
    
    /// <summary>
    /// Performs a complete game reset including deck state and returns to deck selection
    /// </summary>
    private void PerformCompleteGameReset()
    {
        Debug.Log("Starting complete game reset...");
        
        try
        {
            // Clean up any existing gameplay cards first
            CleanupGameplayCards();
            
            // Reset gameplay state
            ResetGameState();
            
            // Request deck reset via event system
            eventManager?.Publish(new GameplayEvents.ResetDeckRequested());
            
            // Request return to deck selection via event system
            eventManager?.Publish(new GameplayEvents.ReturnToDeckSelectionRequested());
            
            Debug.Log("Complete game reset successful");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error during complete game reset: {e.Message}");
        }
    }
    
    private void CleanupGameplayCards()
    {
        if (currentPlayerCard != null)
        {
            cardViewFactory?.DestroyCardView(currentPlayerCard);
            currentPlayerCard = null;
        }
        
        var remainingCards = FindObjectsByType<Card3DView>(FindObjectsSortMode.None);
        foreach (var card in remainingCards)
        {
            cardViewFactory?.DestroyCardView(card);
        }
        
        Debug.Log("Gameplay cards cleanup completed");
    }
    
    /// <summary>
    /// Creates 3D cards for gameplay from selected deck cards
    /// </summary>
    /// <param name="selectedCards">List of selected cards to create 3D versions for</param>
    /// <returns>Task representing the async card creation operation</returns>
    public async Task CreateGameplay3DCards(List<CardData> selectedCards)
    {
        if (selectedCards == null || selectedCards.Count == 0)
        {
            Debug.LogWarning("No selected cards provided for 3D card creation");
            return;
        }

        if (gameplayConfig == null || !gameplayConfig.ArePositionsValid())
        {
            Debug.LogError("Invalid gameplay configuration - cannot create 3D cards");
            return;
        }

        try
        {
            Debug.Log($"Creating {selectedCards.Count} 3D cards for gameplay...");
            
            var card3DViews = await cardViewFactory.CreateCard3DViewsAtPositionsAsync(
                selectedCards, 
                gameplayConfig
            );

            Debug.Log($"Successfully created {card3DViews.Count} 3D cards at designated positions");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create gameplay 3D cards: {e.Message}");
            throw; // Re-throw to allow caller to handle
        }
    }
    
    public void Dispose()
    {
        eventManager?.Unsubscribe<GameplayEvents.UseSkillRequested>(OnUseSkillRequested);
        eventManager?.Unsubscribe<GameplayEvents.EndTurnRequested>(OnEndTurnRequested);
        eventManager?.Unsubscribe<GameplayEvents.RetryGameRequested>(OnRetryGameRequested);
        eventManager?.Unsubscribe<GameplayEvents.CreateGameplay3DCardsRequested>(OnCreateGameplay3DCardsRequested);
    }
}
