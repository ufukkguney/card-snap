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
    
    /// <summary>
    /// Maksimum yerleştirilebilir kart sayısını döndürür
    /// </summary>
    public int MaxGameplayCards => cardPositions?.Count ?? 0;
    
    /// <summary>
    /// Belirtilen index'teki pozisyonu döndürür
    /// </summary>
    public Transform GetCardPosition(int index)
    {
        if (cardPositions == null || index < 0 || index >= cardPositions.Count)
            return null;
        
        return cardPositions[index];
    }
    
    /// <summary>
    /// Tüm pozisyonların geçerli olup olmadığını kontrol eder
    /// </summary>
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
