using UnityEngine;

public abstract class BaseCardView : MonoBehaviour
{
    [Header("Card Data")]
    [SerializeField] protected CardData cardData;
    
    public CardData Data
    {
        get => cardData;
        set
        {
            cardData = value;
            UpdateUI();
        }
    }
    protected virtual void Start()
    {
        UpdateUI();
    }
    
    public void SetCardData(CardData newData)
    {
        Data = ValidateCardData(newData);
    }
    private CardData ValidateCardData(CardData data)
    {
        return new CardData(data.CardType,
            Mathf.Max(0, data.Attack),
            Mathf.Max(0, data.Defense)
        );
    }
    
    protected abstract void UpdateUI();
    
}
