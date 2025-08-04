using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameplayConfiguration
{
    [Header("3D Card Positions")]
    [SerializeField] public List<Transform> cardPositions = new List<Transform>();
    
    [Header("Gameplay Settings")]
    [SerializeField] public float cardInstantiationDelay = 0.2f;
    [SerializeField] public bool animateCardPlacement = true;
    
    [Header("Visual Effects")]
    [SerializeField] public GameObject cardPlacementEffect;
    [SerializeField] public AudioClip cardPlacementSound;
    
    public int MaxGameplayCards => cardPositions?.Count ?? 0;
    
    public Transform GetCardPosition(int index)
    {
        if (cardPositions == null || index < 0 || index >= cardPositions.Count)
            return null;
        
        return cardPositions[index];
    }
    
    public bool ArePositionsValid()
    {
        if (cardPositions == null || cardPositions.Count == 0)
            return false;
        
        foreach (var position in cardPositions)
        {
            if (position == null)
                return false;
        }
        
        return true;
    }
}
