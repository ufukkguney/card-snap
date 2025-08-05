using UnityEngine;
using System.Collections.Generic;
using System;
using VContainer;

public class GamePlayController : MonoBehaviour, IDisposable
{
    public GameplayConfiguration gameplayConfig;
    
    private Dictionary<DropTarget, Card3DView> targetCards = new Dictionary<DropTarget, Card3DView>();
    private EventManager eventManager;
    private AIPlayer aiPlayer;
    
    [Inject]
    public void Construct(EventManager eventManager, AIPlayer aiPlayer)
    {
        this.eventManager = eventManager;
        this.aiPlayer = aiPlayer;
        SubscribeToEvents();
    }
    
    public void OnCardPlacedOnTarget(DropTarget target, Card3DView cardView)
    {
        targetCards[target] = cardView;
        Debug.Log($"Card {cardView.name} placed on {target.name}");
    }
    
    public void OnCardRemovedFromTarget(DropTarget target, Card3DView cardView)
    {
        targetCards.Remove(target);
        Debug.Log($"Card {cardView.name} removed from {target.name}");
    }
    
    private void SubscribeToEvents()
    {
        eventManager.Subscribe<GameplayEvents.UseSkillRequested>(OnUseSkillRequested);
        eventManager.Subscribe<GameplayEvents.EndTurnRequested>(OnEndTurnRequested);
    }
    
    private void OnUseSkillRequested(GameplayEvents.UseSkillRequested evt) => 
        Debug.Log("Use Skill Requested");
    
    private void OnEndTurnRequested(GameplayEvents.EndTurnRequested evt) => 
        _ = CreateAICardAsync();
    
    private async System.Threading.Tasks.Task CreateAICardAsync()
    {
        if (aiPlayer == null || gameplayConfig?.AiDropTarget == null) return;
       
        var aiCard = await aiPlayer.CreateRandomCardViewAsync(gameplayConfig.AiDropTarget.transform);
        if (aiCard != null)
        {
            gameplayConfig.AiDropTarget.PlaceCard(aiCard);
            Debug.Log($"AI created: {aiCard.Data.CardType} at {gameplayConfig.AiDropTarget.name}");
        }
    }
    
    public void Dispose()
    {
        eventManager?.Unsubscribe<GameplayEvents.UseSkillRequested>(OnUseSkillRequested);
        eventManager?.Unsubscribe<GameplayEvents.EndTurnRequested>(OnEndTurnRequested);
    }
}
