using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CardView : BaseCardView, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private Image cardImage;
    
    public System.Action<CardView> CardClicked;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateVisuals();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateVisuals();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OnCardClicked();
    }
    
    protected override void UpdateUI()
    {
        if (cardNameText != null)
            cardNameText.text = cardData.CardName;
            
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
    
    protected override void NotifyCardClicked()
    {
        CardClicked?.Invoke(this);
    }
}
