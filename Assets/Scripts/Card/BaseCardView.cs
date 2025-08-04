using UnityEngine;

public abstract class BaseCardView : MonoBehaviour
{
    [Header("Card Data")]
    [SerializeField] protected CardData cardData;
    
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
        }
    }
    protected virtual void Start()
    {
        UpdateUI();
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
    
    protected abstract void UpdateUI();
    
}
