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
}
