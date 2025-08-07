using UnityEngine;
using VContainer;
using System.Threading.Tasks;

public class GameplayBattleSystem
{
    [Inject] private ISkillService skillService;
    [Inject] private AIPlayer aiPlayer;
    [Inject] private GameplayStateManager stateManager;
    [Inject] private GameplayUIController uiController;
    private GameplayConfiguration gameplayConfig;

    public void Initialize(GameplayConfiguration config)
    {
        gameplayConfig = config;
    }
    public void UseSkill()
    {
        var playerSkill = skillService.GetRandomSkill();
        var aiSkill = skillService.GetRandomSkill();

        skillService.ActivateSkill(playerSkill, true);
        skillService.ActivateSkill(aiSkill, false);

        uiController.DisplaySkills(playerSkill.Description, aiSkill.Description);
    }

    public async void EndTurn()
    {
        uiController.SetButtonsInteractable(false);
        stateManager.CurrentPlayerCard?.SetDraggable(false);
        await ProcessAITurn();
    }

    private async Task ProcessAITurn()
    {
        if (!stateManager.CanContinueGame()) return;

        stateManager.IncrementTurn();
        var aiCard = await aiPlayer.CreateRandomCardViewAsync(gameplayConfig.AiDropTarget.transform);
        
        if (aiCard != null)
        {
            gameplayConfig.AiDropTarget.PlaceCard(aiCard);
            ProcessBattle(stateManager.CurrentPlayerCard, aiCard);

            await Task.Delay(3000);
            stateManager.DestroyBattleCards(stateManager.CurrentPlayerCard, aiCard);

            PrepareNextTurn();
            stateManager.CheckGameOver();
        }
    }

    private void ProcessBattle(Card3DView playerCard, Card3DView aiCard)
    {
        if (playerCard == null || aiCard == null) return;
        
        var (playerDamage, aiDamage) = CalculateDamage(playerCard, aiCard);
        stateManager.ApplyDamage(playerDamage, aiDamage);
        uiController.UpdateHealthDisplay(stateManager.PlayerHealth, stateManager.AIHealth);
    }

    private (int playerDamage, int aiDamage) CalculateDamage(Card3DView playerCard, Card3DView aiCard)
    {
        var playerAttack = skillService.GetModifiedAttack(playerCard.Data.Attack, true);
        var playerDefense = skillService.GetModifiedDefense(playerCard.Data.Defense, true);
        var aiAttack = skillService.GetModifiedAttack(aiCard.Data.Attack, false);
        var aiDefense = skillService.GetModifiedDefense(aiCard.Data.Defense, false);
        
        return (Mathf.Max(0, playerAttack - aiDefense), Mathf.Max(0, aiAttack - playerDefense));
    }

    private void PrepareNextTurn()
    {
        skillService.ResetTurnEffects();
        uiController.PrepareNextTurn();
    }
}