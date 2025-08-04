using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DeckConfiguration
{
    [Header("Transform References")]
    public Transform originalCardsParent;
    public Transform selectedCardsParent;
    
    [Header("Selection Settings")]
    public int maxSelectedCards = 5;
    public bool allowDuplicateSelection = false;
    
    [Header("UI References")]
    public Button StartButton; // Max selection'da aktif olacak button
}
