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
        UpdateVisuals();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Card clicked: {cardData.CardType} (ATK: {cardData.Attack}, DEF: {cardData.Defense})");
        
        // EventManager üzerinden card click event'ini publish et
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
    
    protected override void UpdateVisuals()
    {
        if (cardImage != null)
        {
            cardImage.color = GetCurrentColor();
        }
    }
    
    public override void HighlightCard(bool highlight)
    {
        if (cardImage != null)
        {
            cardImage.color = highlight ? Color.green : GetCurrentColor();
        }
    }
    
}
