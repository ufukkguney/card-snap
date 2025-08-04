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
    
    [System.Serializable]
    public struct Reset : EventManager.IGameEvent
    {
        public int NewDeckSize { get; }
        
        public Reset(int newDeckSize)
        {
            NewDeckSize = newDeckSize;
        }
    }
    
    [System.Serializable]
    public struct Empty : EventManager.IGameEvent
    {
        public int LastCardCount { get; }
        
        public Empty(int lastCardCount)
        {
            LastCardCount = lastCardCount;
        }
    }
    
    #endregion
}
