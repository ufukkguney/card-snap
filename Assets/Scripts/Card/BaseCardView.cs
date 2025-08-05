using UnityEngine;

public abstract class BaseCardView : MonoBehaviour
{
    [Header("Card Data")]
    [SerializeField] protected CardData cardData;
    
    protected bool isSelected = false;
    
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
    
    protected abstract void UpdateUI();
    
}
