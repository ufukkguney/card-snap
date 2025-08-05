public static class CardEvents
{
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
}
