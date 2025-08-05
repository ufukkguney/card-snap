using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "CardSO", menuName = "Scriptable Objects/CardSO")]
public class CardSO : ScriptableObject
{
    [Header("Card Collection")]
    [SerializeField] private List<CardData> cards = new List<CardData>();
    
    public List<CardData> Cards => cards;
    public int CardCount => cards.Count;
   
    public bool ContainsCard(CardData cardData)
    {
        return cards.Contains(cardData);
    }
    
    public CardData GetRandomCard()
    {
        if (cards.Count == 0)
        {
            Debug.LogWarning("Cannot get random card from empty collection.");
            return default(CardData);
        }
        
        int randomIndex = Random.Range(0, cards.Count);
        return cards[randomIndex];
    }
    
    public List<CardData> GetRandomCards(int count)
    {
        if (count > cards.Count)
        {
            Debug.LogWarning($"Requested {count} cards but collection only has {cards.Count} cards.");
            count = cards.Count;
        }
        
        var shuffledCards = cards.OrderBy(x => Random.value).ToList();
        return shuffledCards.Take(count).ToList();
    }

}
