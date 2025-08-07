using VContainer;

public class GameplayUIController
{
    [Inject] private GameplayUI gameplayUI;

    public void ResetUI()
    {
        ClearSkillDisplay();
        HideGameFinishedPanel();
        SetButtonsInteractable(true);
    }

    public void PrepareNextTurn()
    {
        StartTimer();
        ClearSkillDisplay();
        SetButtonsInteractable(true);
    }

    public void UpdateHealthDisplay(int playerHealth, int aiHealth) =>
        gameplayUI?.UpdateHealthDisplay(playerHealth, aiHealth);

    public void DisplaySkills(string playerSkill, string aiSkill) =>
        gameplayUI?.DisplaySkills(playerSkill, aiSkill);

    public void StartTimer() =>
        gameplayUI?.StartTimer();

    public void SetButtonsInteractable(bool interactable) =>
        gameplayUI?.SetButtonsInteractable(interactable);

    public void ShowGameFinishedPanel(string message) =>
        gameplayUI?.ShowGameFinishedPanel(message);

    public void HideGameFinishedPanel() =>
        gameplayUI?.HideGameFinishedPanel();
        
    private void ClearSkillDisplay() =>
    gameplayUI?.ClearSkillDisplay();
    
}