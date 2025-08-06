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
    private ISkillService skillService;
    private int playerHealth = 200;
    private int aiPlayerHealth = 200;

    private int PlayerHealth
    {
        get => playerHealth;
        set => playerHealth = Mathf.Max(0, value);
    }

    private int AiPlayerHealth
    {
        get => aiPlayerHealth;
        set => aiPlayerHealth = Mathf.Max(0, value);
    }
    private int currentTurn = 0;
    private const int MAX_TURNS = 6;
    
    [Inject]
    public void Construct(EventManager eventManager, AIPlayer aiPlayer, GameplayUI gameplayUI, CardViewFactory cardViewFactory, ISkillService skillService)
    {
        (this.eventManager, this.aiPlayer, this.gameplayUI, this.cardViewFactory, this.skillService) = (eventManager, aiPlayer, gameplayUI, cardViewFactory, skillService);
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
    
    private void OnUseSkillRequested(GameplayEvents.UseSkillRequested evt)
    {
        var playerSkill = skillService.GetRandomSkill();
        var aiSkill = skillService.GetRandomSkill();
        
        skillService.ActivateSkill(playerSkill, true);  // Player skill
        skillService.ActivateSkill(aiSkill, false);     // AI skill
        
        gameplayUI?.DisplaySkills(playerSkill.description, aiSkill.description);
        Debug.Log($"Skills activated - Player: {playerSkill.description}, AI: {aiSkill.description}");
    }
    
    private void OnEndTurnRequested(GameplayEvents.EndTurnRequested evt)
    {
        gameplayUI?.SetButtonsInteractable(false);
        
        _ = CreateAICardAsync();
    }
    
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
            gameplayUI?.StartTimer();
            skillService.ResetTurnEffects();
            gameplayUI?.ClearSkillDisplay();
            gameplayUI?.SetButtonsInteractable(true);

            CheckGameOver();
        }
    }
    
    private void ProcessBattle(Card3DView playerCard, Card3DView aiCard)
    {
        if (playerCard == null || aiCard == null) return;
        
        var playerAttack = skillService.GetModifiedAttack(playerCard.Data.Attack, true);
        var playerDefense = skillService.GetModifiedDefense(playerCard.Data.Defense, true);
        var aiAttack = skillService.GetModifiedAttack(aiCard.Data.Attack, false);
        var aiDefense = skillService.GetModifiedDefense(aiCard.Data.Defense, false);
        
        var playerDamage = Mathf.Max(0, playerAttack - aiDefense);
        var aiDamage = Mathf.Max(0, aiAttack - playerDefense);
        
        skillService.ProcessHealthEffects(ref playerHealth, ref aiDamage, true);
        skillService.ProcessHealthEffects(ref aiPlayerHealth, ref playerDamage, false);
        
        AiPlayerHealth -= playerDamage;
        PlayerHealth -= aiDamage;
        
        UpdateHealthDisplay();
        
        Debug.Log($"Battle: Player dealt {playerDamage} damage, AI dealt {aiDamage} damage");
    }
    
    private void UpdateHealthDisplay() => gameplayUI?.UpdateHealthDisplay(playerHealth, aiPlayerHealth);
    
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
        
        if (sb.Length > 0)
        {
            gameplayUI?.ShowGameFinishedPanel(sb.ToString());
            gameplayUI?.SetButtonsInteractable(false); // Disable buttons when game ends
        }
    }
    
    private void DestroyBattleCards(Card3DView playerCard, Card3DView aiCard)
    {
        cardViewFactory?.DestroyCardView(playerCard);
        cardViewFactory?.DestroyCardView(aiCard);
        currentPlayerCard = null;
    }
    
    public void ResetGameState()
    {
        PlayerHealth = 200;
        AiPlayerHealth = 200;
        currentTurn = 0;
        currentPlayerCard = null;
        
        skillService?.ResetTurnEffects();
        gameplayUI?.ClearSkillDisplay();
        gameplayUI?.HideGameFinishedPanel();
        gameplayUI?.SetButtonsInteractable(true); // Re-enable buttons on reset

        UpdateHealthDisplay();
        
        Debug.Log("GamePlay state reset to initial values");
    }
    
    private void PerformCompleteGameReset()
    {
        Debug.Log("Starting complete game reset...");
        
        CleanupGameplayCards();
        ResetGameState();
        
        eventManager?.Publish(new GameplayEvents.ResetDeckRequested());
        eventManager?.Publish(new GameplayEvents.ReturnToDeckSelectionRequested());
        
        Debug.Log("Complete game reset successful");
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
    
    public async Task CreateGameplay3DCards(List<CardData> selectedCards)
    {
        var card3DViews = await cardViewFactory.CreateCard3DViewsAtPositionsAsync(
            selectedCards, 
            gameplayConfig
        );

        Debug.Log($"Successfully created {card3DViews.Count} 3D cards at designated positions");
    }
    
    public void Dispose()
    {
        eventManager?.Unsubscribe<GameplayEvents.UseSkillRequested>(OnUseSkillRequested);
        eventManager?.Unsubscribe<GameplayEvents.EndTurnRequested>(OnEndTurnRequested);
        eventManager?.Unsubscribe<GameplayEvents.RetryGameRequested>(OnRetryGameRequested);
        eventManager?.Unsubscribe<GameplayEvents.CreateGameplay3DCardsRequested>(OnCreateGameplay3DCardsRequested);
    }
}
