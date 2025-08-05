using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class GameplayUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button useSkillButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private TextMeshProUGUI aiHealthText;
    [SerializeField] private TextMeshProUGUI gameFinishedText;
    [SerializeField] private GameObject gameFinishedPanel;
    private EventManager eventManager;

    [Inject]
    public void Construct(EventManager eventManager) => this.eventManager = eventManager;

    private void Start() => SetupButtons();

    private void SetupButtons()
    {
        useSkillButton?.onClick.AddListener(() => eventManager?.Publish(new GameplayEvents.UseSkillRequested()));
        endTurnButton?.onClick.AddListener(() => eventManager?.Publish(new GameplayEvents.EndTurnRequested()));
    }

    public void UpdateHealthDisplay(int playerHealth, int aiHealth)
    {
        if (playerHealthText != null) playerHealthText.text = $"Player: {playerHealth}";
        if (aiHealthText != null) aiHealthText.text = $"AI: {aiHealth}";
    }

    private void OnDestroy()
    {
        useSkillButton?.onClick.RemoveAllListeners();
        endTurnButton?.onClick.RemoveAllListeners();
    }
    
    public void ShowGameFinishedPanel(string message)
    {
        if (gameFinishedPanel != null)
        {
            gameFinishedPanel.SetActive(true);
            gameFinishedText.text = message;
        }
    }
}
