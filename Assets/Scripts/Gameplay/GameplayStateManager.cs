using Unity.Mathematics;
using UnityEngine;
using VContainer;

public class GameplayStateManager
{
    [Inject] private ICardViewFactory cardViewFactory;
    [Inject] private GameplayUIController uiController;
    [Inject] private EventManager eventManager;
    [Inject] private ISkillService skillService;

    public Card3DView CurrentPlayerCard { get; private set; }

    private int playerHealth;
    private int aiHealth;
    public int PlayerHealth
    {
        get => playerHealth;
        private set => playerHealth = Mathf.Max(value, 0);
    }
    public int AIHealth 
    { 
        get => aiHealth; 
        private set => aiHealth = Mathf.Max(value, 0); 
    }
    public int CurrentTurn { get; private set; }
    
    private const int MAX_TURNS = 6;

    public void Initialize() => ResetToInitialState();
    public void SetCurrentPlayerCard(Card3DView card) => CurrentPlayerCard = card;
    public void IncrementTurn() => CurrentTurn++;
    public bool CanContinueGame() => CurrentTurn < MAX_TURNS && playerHealth > 0 && aiHealth > 0;

    public void ApplyDamage(int playerDamage, int aiDamage)
    {
        skillService.ProcessHealthEffects(ref playerHealth, ref aiDamage, true);
        skillService.ProcessHealthEffects(ref aiHealth, ref playerDamage, false);
        
        aiHealth -= playerDamage;
        playerHealth -= aiDamage;
    }

    public void CheckGameOver()
    {
        var message = GetGameOverMessage();
        if (!string.IsNullOrEmpty(message))
        {
            uiController.ShowGameFinishedPanel(message);
            uiController.SetButtonsInteractable(false);
        }
    }

    private string GetGameOverMessage()
    {
        if (playerHealth <= 0) return "AI WINS!\nPlayer health reached 0";
        if (aiHealth <= 0) return "PLAYER WINS!\nAI health reached 0";
        if (CurrentTurn >= MAX_TURNS)
        {
            if (playerHealth > aiHealth) return $"PLAYER WINS!\n{playerHealth} vs {aiHealth} health after {MAX_TURNS} turns";
            if (aiHealth > playerHealth) return $"AI WINS!\n{aiHealth} vs {playerHealth} health after {MAX_TURNS} turns";
            return $"DRAW!\nBoth have {playerHealth} health after {MAX_TURNS} turns";
        }
        return null;
    }

    public void DestroyBattleCards(Card3DView playerCard, Card3DView aiCard)
    {
        cardViewFactory?.DestroyCardView(playerCard);
        cardViewFactory?.DestroyCardView(aiCard);
        CurrentPlayerCard = null;
    }

    public void ResetGame()
    {
        CleanupCards();
        ResetToInitialState();
        eventManager?.Publish(new GameplayEvents.ResetDeckRequested());
        eventManager?.Publish(new GameplayEvents.ReturnToDeckSelectionRequested());
    }

    private void ResetToInitialState()
    {
        playerHealth = aiHealth = 200;
        CurrentTurn = 0;
        CurrentPlayerCard = null;
        skillService?.ResetTurnEffects();
        uiController?.ResetUI();
        uiController?.UpdateHealthDisplay(playerHealth, aiHealth);
    }

    private void CleanupCards()
    {
        if (CurrentPlayerCard != null)
        {
            cardViewFactory?.DestroyCardView(CurrentPlayerCard);
            CurrentPlayerCard = null;
        }
        
        var remainingCards = Object.FindObjectsByType<Card3DView>(FindObjectsSortMode.None);
        foreach (var card in remainingCards)
            cardViewFactory?.DestroyCardView(card);
    }
}