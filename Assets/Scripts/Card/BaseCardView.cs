using UnityEngine;

public abstract class BaseCardView : MonoBehaviour
{
    [Header("Card Data")]
    [SerializeField] protected CardData cardData;
    
    [Header("Visual Settings")]
    [SerializeField] protected Color normalColor = Color.white;
    [SerializeField] protected Color selectedColor = Color.yellow;
    [SerializeField] protected Color hoveredColor = Color.cyan;
    
    protected bool isSelected = false;
    protected bool isHovered = false;
    
    
    public CardData Data
    {
        get => cardData;
        set
        {
            cardData = value;
            UpdateUI();
        }
    }
    
    public bool IsSelected 
    { 
        get => isSelected;
        set 
        { 
            isSelected = value;
            UpdateVisuals();
        }
    }
    protected virtual void Start()
    {
        UpdateUI();
        UpdateVisuals();
    }
    
    public void SetCardData(CardData newData)
    {
        Data = newData;
    }
    
    public void SetCardData(CardType type, int attack, int defense)
    {
        Data = new CardData(type, attack, defense);
    }
    
    public virtual void PlayCard()
    {
        Debug.Log($"Playing card: {cardData.CardType} (ATK: {cardData.Attack}, DEF: {cardData.Defense})");
    }
    
    public abstract void HighlightCard(bool highlight);
    
    protected Color GetCurrentColor()
    {
        if (isSelected) return selectedColor;
        if (isHovered) return hoveredColor;
        return normalColor;
    }
    
    protected virtual void OnCardClicked()
    {
        Debug.Log($"Card clicked: {cardData.CardType}");
        IsSelected = !IsSelected;
        NotifyCardClicked();
    }
    
    protected abstract void UpdateUI();
    protected abstract void UpdateVisuals();
    protected abstract void NotifyCardClicked();
    
    protected virtual void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdateUI();
        }
    }
}
