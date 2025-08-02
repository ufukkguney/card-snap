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
    
    public void AddCard(CardData cardData)
    {
        cards.Add(cardData);
    }
    
    public void AddCard(CardType type, int attack, int defense)
    {
        cards.Add(new CardData(type, attack, defense));
    }
    
    public bool RemoveCard(CardData cardData)
    {
        return cards.Remove(cardData);
    }
    
    public bool RemoveCardByType(CardType cardType)
    {
        var cardToRemove = cards.FirstOrDefault(card => card.CardType == cardType);
        if (cardToRemove.CardType != default(CardType))
        {
            return cards.Remove(cardToRemove);
        }
        return false;
    }
    
    public CardData GetCard(int index)
    {
        if (index >= 0 && index < cards.Count)
        {
            return cards[index];
        }
        
        Debug.LogWarning($"Card index {index} out of range. Collection has {cards.Count} cards.");
        return default(CardData);
    }
    
    public bool ContainsCard(CardData cardData)
    {
        return cards.Contains(cardData);
    }
    
    public void ClearCards()
    {
        cards.Clear();
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

    public void PrintAllCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Debug.Log($"{i}: {cards[i].ToString()}");
        }
    }
    
    private void OnValidate()
    {
        if (cards != null)
        {
            var distinctCards = cards.GroupBy(card => card.CardType)
                                   .Select(g => g.First())
                                   .ToList();
            
            if (distinctCards.Count != cards.Count && cards.Count > 0)
            {
                Debug.LogWarning($"Duplicate cards detected in {name}. Consider removing duplicates.");
            }
        }
    }
}
