using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager
{
    private readonly Dictionary<Type, List<Delegate>> eventListeners = new();
    public interface IGameEvent { }

    public void Subscribe<T>(Action<T> listener) where T : IGameEvent
    {
        var eventType = typeof(T);
        
        if (!eventListeners.ContainsKey(eventType))
            eventListeners[eventType] = new List<Delegate>();
        
        eventListeners[eventType].Add(listener);
    }
    
    public void Unsubscribe<T>(Action<T> listener) where T : IGameEvent
    {
        var eventType = typeof(T);
        
        if (eventListeners.TryGetValue(eventType, out var listeners))
        {
            listeners.Remove(listener);
            if (listeners.Count == 0)
                eventListeners.Remove(eventType);
        }
    }

    public void Publish<T>(T eventArgs) where T : IGameEvent
    {
        var eventType = typeof(T);
        
        if (!eventListeners.TryGetValue(eventType, out var listeners)) return;
        
        var listenersCopy = new List<Delegate>(listeners);
        
        foreach (var listener in listenersCopy)
        {
            try { ((Action<T>)listener).Invoke(eventArgs); }
            catch (Exception ex) { Debug.LogError($"Event error in {eventType.Name}: {ex.Message}"); }
        }
    }
    
    public void PublishCardClicked(BaseCardView cardView, CardData cardData) 
        => Publish(new CardEvents.Clicked(cardView, cardData));
    
    
    public void ClearAllListeners()
    {
        var totalCleared = eventListeners.Values.Sum(list => list.Count);
        eventListeners.Clear();
        Debug.Log($"Cleared {totalCleared} event listeners");
    }
}