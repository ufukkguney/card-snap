using System.Collections.Generic;

public static class DeckEvents
{
    #region Deck Lifecycle Events
    
    [System.Serializable]
    public struct Initialized : EventManager.IGameEvent
    {
        public int DeckSize { get; }
        public List<CardData> DeckCards { get; }
        
        public Initialized(int deckSize, List<CardData> deckCards)
        {
            DeckSize = deckSize;
            DeckCards = new List<CardData>(deckCards);
        }
    }
    
    #endregion
}
