using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DeckConfiguration
{
    [Header("Transform References")]
    public Transform originalCardsParent;
    public Transform selectedCardsParent;
    public Transform deckAreaParent;
    public Transform gameplayAreaParent;
    
    [Header("Selection Settings")]
    public int maxSelectedCards = 5;
    public bool allowDuplicateSelection = false;
    
    [Header("UI References")]
    public Button StartButton; // Max selection'da aktif olacak button
    
    // [Header("Gameplay Configuration")]
    // public GameplayConfiguration gameplayConfig;
}
