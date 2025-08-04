using System.Collections.Generic;
using System.Linq;

public class SelectionManager
{
    private readonly Deck deck;
    private readonly EventManager eventManager;
    private readonly DeckConfiguration config;

    public SelectionManager(Deck deck, EventManager eventManager, DeckConfiguration config)
    {
        this.deck = deck;
        this.eventManager = eventManager;
        this.config = config;
    }

    public bool TryAddSelection(CardData cardData)
    {
        if (GetSelectedCount() >= config.maxSelectedCards)
        {
            eventManager.PublishMaxSelectionReached(config.maxSelectedCards, cardData);
            return false;
        }

        if (!config.allowDuplicateSelection && IsCardSelected(cardData))
            return false;

        deck.AddSelectedCard(cardData);
        eventManager.PublishCardSelectionChanged(cardData, true, GetSelectedCount());
        return true;
    }

    public bool TryRemoveSelection(CardData cardData)
    {
        if (!IsCardSelected(cardData)) return false;

        deck.RemoveSelectedCard(cardData);
        eventManager.PublishCardSelectionChanged(cardData, false, GetSelectedCount());
        return true;
    }

    public void ClearAllSelections()
    {
        var selectedCards = deck.SelectedCards.ToList();
        deck.ClearSelectedCards();
        eventManager.PublishAllSelectionsCleared(selectedCards);
    }

    public bool IsCardSelected(CardData cardData) => deck?.SelectedCards.Contains(cardData) ?? false;
    public int GetSelectedCount() => deck?.SelectedCardCount ?? 0;
    public bool IsMaxReached => GetSelectedCount() >= config.maxSelectedCards;
    public List<CardData> SelectedCards => deck?.SelectedCards.ToList() ?? new List<CardData>();
}
