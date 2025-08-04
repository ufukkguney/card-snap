using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Deck
{
    private int deckSize = 10;
    
    private List<CardData> deckCards = new List<CardData>();
    private List<CardData> selectedCards = new List<CardData>();

    private CardSO cardSO;
    private EventManager eventManager;

    public List<CardData> DeckCards => deckCards;
    public List<CardData> SelectedCards => selectedCards;
    public int DeckSize => deckSize;
    public int SelectedCardCount => selectedCards.Count;

    public Deck(CardSO cardSO = null, EventManager eventManager = null)
    {
        this.cardSO = cardSO;
        this.eventManager = eventManager;
    }

    public void InitializeDeck()
    {
        deckCards.Clear();
        
        if (cardSO == null)
        {
            Debug.LogError("CardSO is not available! Cannot initialize deck.");
            return;
        }

        if (cardSO.CardCount == 0)
        {
            Debug.LogWarning("CardSO has no cards! Cannot initialize deck.");
            return;
        }

        GenerateRandomCards();
        
        eventManager?.PublishDeckInitialized(deckSize, deckCards);
    }

    private void GenerateRandomCards()
    {
        List<CardData> randomCards = cardSO.GetRandomCards(deckSize);
        deckCards.AddRange(randomCards);
    }

    public void AddSelectedCard(CardData cardData)
    {
        if (!selectedCards.Contains(cardData))
        {
            selectedCards.Add(cardData);
            Debug.Log($"Card selected: {cardData.CardType} (ATK: {cardData.Attack}, DEF: {cardData.Defense})");
        }
        else
        {
            Debug.LogWarning("Card is already selected!");
        }
    }

    public void RemoveSelectedCard(CardData cardData)
    {
        if (selectedCards.Remove(cardData))
        {
            Debug.Log($"Card removed from selection: {cardData.CardType}");
        }
        else
        {
            Debug.LogWarning("Card was not in selected cards!");
        }
    }

    public void ClearSelectedCards()
    {
        selectedCards.Clear();
        Debug.Log("All selected cards cleared.");
    }
    
    public CardData GetDeckCard(int index)
    {
        if (index >= 0 && index < deckCards.Count)
        {
            return deckCards[index];
        }

        Debug.LogWarning($"Deck card index {index} out of range. Deck has {deckCards.Count} cards.");
        return default(CardData);
    }

    public void ResetDeck()
    {
        selectedCards.Clear();
        InitializeDeck();
        Debug.Log("Deck reset to original state.");
    }

    public List<CardData> GetCardsByType(CardType cardType)
    {
        return deckCards.Where(card => card.CardType == cardType).ToList();
    }

}
