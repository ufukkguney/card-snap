using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class GameplayUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button useSkillButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private TextMeshProUGUI aiHealthText;
    [SerializeField] private TextMeshProUGUI gameFinishedText;
    [SerializeField] private TextMeshProUGUI playerSkillText;
    [SerializeField] private TextMeshProUGUI aiSkillText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject gameFinishedPanel;

    private EventManager eventManager;
    private const float TIMER_DURATION = 10f;
    private float currentTime;
    private bool isTimerRunning;

    [Inject]
    public void Construct(EventManager eventManager)
    {
        this.eventManager = eventManager;
        SetupButtons();
    }

    private void Update()
    {
        if (!isTimerRunning) return;
        
        currentTime += Time.deltaTime;
        float remainingTime = Mathf.Max(0, TIMER_DURATION - currentTime);
        UpdateTimerDisplay(remainingTime);
        
        if (currentTime >= TIMER_DURATION)
        {
            EndTurn();
        }
    }

    private void SetupButtons()
    {
        useSkillButton?.onClick.AddListener(OnUseSkillButtonClicked);
        endTurnButton?.onClick.AddListener(EndTurn);
        retryButton?.onClick.AddListener(() => eventManager?.Publish(new GameplayEvents.RetryGameRequested()));
    }
    private void OnUseSkillButtonClicked()
    {
        eventManager?.Publish(new GameplayEvents.UseSkillRequested());
        useSkillButton.interactable = false;
    }

    public void UpdateHealthDisplay(int playerHealth, int aiHealth)
    {
        playerHealthText?.SetText($"Player: {playerHealth}");
        aiHealthText?.SetText($"AI: {aiHealth}");
    }

    public void ShowGameFinishedPanel(string message)
    {
        gameFinishedPanel?.SetActive(true);
        gameFinishedText?.SetText(message);
        StopTimer();
    }

    public void HideGameFinishedPanel() => gameFinishedPanel?.SetActive(false);

    public void DisplaySkills(string playerSkill, string aiSkill)
    {
        playerSkillText?.SetText($"Player Skill: {playerSkill}");
        aiSkillText?.SetText($"AI Skill: {aiSkill}");
    }

    public void ClearSkillDisplay()
    {
        playerSkillText?.SetText("");
        aiSkillText?.SetText("");
    }

    public void SetButtonsInteractable(bool interactable)
    {
        if (useSkillButton) useSkillButton.interactable = interactable;
        if (endTurnButton) endTurnButton.interactable = interactable;
    }

    public void StartTimer()
    {
        currentTime = 0f;
        isTimerRunning = true;
        UpdateTimerDisplay(TIMER_DURATION);
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        currentTime = 0f;
        UpdateTimerDisplay(0f);
    }

    private void UpdateTimerDisplay(float remainingTime) => 
        timerText?.SetText($"{Mathf.CeilToInt(remainingTime)}s");

    private void EndTurn()
    {
        eventManager?.Publish(new GameplayEvents.EndTurnRequested());
        StopTimer();
    }

    private void OnDestroy()
    {
        useSkillButton?.onClick.RemoveAllListeners();
        endTurnButton?.onClick.RemoveAllListeners();
        retryButton?.onClick.RemoveAllListeners();
    }
}
