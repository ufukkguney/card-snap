using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using VContainer;

public class CardUIView : BaseCardView, IPointerClickHandler
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private Image cardImage;

    [Inject] private EventManager eventManager;

    public void Initialize()
    {
        UpdateUI();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Card clicked: {cardData.CardType} (ATK: {cardData.Attack}, DEF: {cardData.Defense})");
        
        eventManager?.PublishCardClicked(this, cardData);
    }
    
    protected override void UpdateUI()
    {
        if (cardNameText != null)
            cardNameText.text = cardData.CardType.ToString();
            
        if (attackText != null)
            attackText.text = cardData.Attack.ToString();
            
        if (defenseText != null)
            defenseText.text = cardData.Defense.ToString();
    }
}
