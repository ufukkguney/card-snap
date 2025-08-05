using UnityEngine;
using System;
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
    }
    
    private void OnUseSkillRequested(GameplayEvents.UseSkillRequested evt) =>
        Debug.Log("Use Skill Requested");
    
    private void OnEndTurnRequested(GameplayEvents.EndTurnRequested evt) =>
        _ = CreateAICardAsync();
    
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
    
    public void Dispose()
    {
        eventManager?.Unsubscribe<GameplayEvents.UseSkillRequested>(OnUseSkillRequested);
        eventManager?.Unsubscribe<GameplayEvents.EndTurnRequested>(OnEndTurnRequested);
    }
}
