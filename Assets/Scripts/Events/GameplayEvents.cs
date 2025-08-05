public static class GameplayEvents
{
    [System.Serializable]
    public struct UseSkillRequested : EventManager.IGameEvent
    {
    }
    
    [System.Serializable]
    public struct EndTurnRequested : EventManager.IGameEvent
    {
    }
    
    [System.Serializable]
    public struct RetryGameRequested : EventManager.IGameEvent
    {
    }
    
    [System.Serializable]
    public struct ResetDeckRequested : EventManager.IGameEvent
    {
    }
    
    [System.Serializable]
    public struct ReturnToDeckSelectionRequested : EventManager.IGameEvent
    {
    }
    
    [System.Serializable]
    public struct CreateGameplay3DCardsRequested : EventManager.IGameEvent
    {
        public System.Collections.Generic.List<CardData> SelectedCards { get; }
        
        public CreateGameplay3DCardsRequested(System.Collections.Generic.List<CardData> selectedCards)
        {
            SelectedCards = selectedCards;
        }
    }
}
