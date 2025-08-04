using System.Collections.Generic;

public static class CardEvents
{
    #region Card Interaction Events
    
    [System.Serializable]
    public struct Clicked : EventManager.IGameEvent
    {
        public BaseCardView CardView { get; }
        public CardData CardData { get; }
        
        public Clicked(BaseCardView cardView, CardData cardData)
        {
            CardView = cardView;
            CardData = cardData;
        }
    }
    
    #endregion
    
    #region Card Selection Events
    
    [System.Serializable]
    public struct SelectionChanged : EventManager.IGameEvent
    {
        public CardData CardData { get; }
        public bool IsSelected { get; }
        public int TotalSelectedCount { get; }
        
        public SelectionChanged(CardData cardData, bool isSelected, int totalSelectedCount)
        {
            CardData = cardData;
            IsSelected = isSelected;
            TotalSelectedCount = totalSelectedCount;
        }
    }
    
    [System.Serializable]
    public struct AllSelectionsCleared : EventManager.IGameEvent
    {
        public List<CardData> PreviouslySelectedCards { get; }
        
        public AllSelectionsCleared(List<CardData> previouslySelectedCards)
        {
            PreviouslySelectedCards = new List<CardData>(previouslySelectedCards);
        }
    }
    
    [System.Serializable]
    public struct MaxSelectionReached : EventManager.IGameEvent
    {
        public int MaxSelections { get; }
        public CardData AttemptedCard { get; }
        
        public MaxSelectionReached(int maxSelections, CardData attemptedCard)
        {
            MaxSelections = maxSelections;
            AttemptedCard = attemptedCard;
        }
    }
    
    #endregion
}
