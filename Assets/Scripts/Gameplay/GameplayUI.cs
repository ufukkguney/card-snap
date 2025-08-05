using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class GameplayUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button useSkillButton;
    [SerializeField] private Button endTurnButton;
    
    private EventManager eventManager;
    
    [Inject]
    public void Construct(EventManager eventManager) => this.eventManager = eventManager;
    
    private void Start() => SetupButtons();
    
    private void SetupButtons()
    {
        useSkillButton?.onClick.AddListener(() => eventManager?.Publish(new GameplayEvents.UseSkillRequested()));
        endTurnButton?.onClick.AddListener(() => eventManager?.Publish(new GameplayEvents.EndTurnRequested()));
    }
    
    private void OnDestroy()
    {
        useSkillButton?.onClick.RemoveAllListeners();
        endTurnButton?.onClick.RemoveAllListeners();
    }
}
