using System.Collections.Generic;
using System.Linq;

public class Deck
{
    private const int DECK_SIZE = 10;
    private List<CardData> deckCards = new List<CardData>();
    private List<CardData> selectedCards = new List<CardData>();
    private CardSO cardSO;

    public List<CardData> DeckCards => deckCards;
    public List<CardData> SelectedCards => selectedCards;
    public int SelectedCardCount => selectedCards.Count;

    public Deck(CardSO cardSO = null) => this.cardSO = cardSO;

    public void InitializeDeck()
    {
        deckCards.Clear();
        if (cardSO?.CardCount > 0) deckCards.AddRange(cardSO.GetRandomCards(DECK_SIZE));
    }

    public void AddSelectedCard(CardData cardData)
    {
        if (!selectedCards.Contains(cardData)) selectedCards.Add(cardData);
    }

    public void RemoveSelectedCard(CardData cardData) => selectedCards.Remove(cardData);

    public void ClearSelectedCards() => selectedCards.Clear();
    
    public CardData GetDeckCard(int index) => 
        index >= 0 && index < deckCards.Count ? deckCards[index] : default(CardData);

    public void ResetDeck()
    {
        selectedCards.Clear();
        InitializeDeck();
    }

    public List<CardData> GetCardsByType(CardType cardType) => 
        deckCards.Where(card => card.CardType == cardType).ToList();
}
