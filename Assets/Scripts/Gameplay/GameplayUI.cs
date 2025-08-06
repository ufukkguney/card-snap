using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

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

    private void SetupButtons()
    {
        useSkillButton?.onClick.AddListener(() => eventManager?.Publish(new GameplayEvents.UseSkillRequested()));
        endTurnButton?.onClick.AddListener(() => {
            TriggerEndTurn();
            StopTimer();
        });
        retryButton?.onClick.AddListener(() => eventManager?.Publish(new GameplayEvents.RetryGameRequested()));
    }

    public void UpdateHealthDisplay(int playerHealth, int aiHealth)
    {
        if (playerHealthText != null) playerHealthText.text = $"Player: {playerHealth}";
        if (aiHealthText != null) aiHealthText.text = $"AI: {aiHealth}";
    }

    public void ShowGameFinishedPanel(string message)
    {
        if (gameFinishedPanel != null)
        {
            gameFinishedPanel.SetActive(true);
            gameFinishedText.text = message;
        }
        StopTimer();
    }
    
    public void HideGameFinishedPanel()
    {
        if (gameFinishedPanel != null)
        {
            gameFinishedPanel.SetActive(false);
        }
    }
    
    public void DisplaySkills(string playerSkill, string aiSkill)
    {
        if (playerSkillText != null) playerSkillText.text = $"Player Skill: {playerSkill}";
        if (aiSkillText != null) aiSkillText.text = $"AI Skill: {aiSkill}";
    }
    
    public void ClearSkillDisplay()
    {
        if (playerSkillText != null) playerSkillText.text = "";
        if (aiSkillText != null) aiSkillText.text = "";
    }
    
    public void SetButtonsInteractable(bool interactable)
    {
        if (useSkillButton != null) useSkillButton.interactable = interactable;
        if (endTurnButton != null) endTurnButton.interactable = interactable;
    }
    
    public void Update()
    {
        if (!isTimerRunning) return;

        currentTime += Time.deltaTime;
        Debug.Log("Starting timer tick");
        
        float remainingTime = Mathf.Max(0, TIMER_DURATION - currentTime);
        UpdateTimerDisplay(remainingTime);
        
        if (currentTime >= TIMER_DURATION)
        {
            TriggerEndTurn();
            StopTimer();
        }
    }
    
    public void StartTimer()
    {
        Debug.Log("Starting timer");
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
    
    private void UpdateTimerDisplay(float remainingTime)
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(remainingTime);
            timerText.text = $"Time: {seconds}s";
        }
    }
  
    
    private void TriggerEndTurn()
    {
        eventManager?.Publish(new GameplayEvents.EndTurnRequested());
    }

    private void OnDestroy()
    {
        useSkillButton?.onClick.RemoveAllListeners();
        endTurnButton?.onClick.RemoveAllListeners();
        retryButton?.onClick.RemoveAllListeners();
    }
}
