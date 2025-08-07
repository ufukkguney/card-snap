using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DeckConfiguration
{
    [Header("Transform References")]
    public Transform OriginalCardsParent;
    public Transform SelectedCardsParent;
    public Transform DeckAreaParent;
    public Transform GameplayAreaParent;

    [Header("Selection Settings")]
    public int MaxSelectedCards = 6;
    public bool AllowDuplicateSelection = false;

    [Header("UI References")]
    public Button StartButton; 
    
}
