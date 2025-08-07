using System.Collections.Generic;
using System.Linq;
using VContainer;

public class DeckSelectionManager
{
    [Inject] private readonly Deck deck;
    private DeckConfiguration config;

    public void Initialize(DeckConfiguration config)
    {
        this.config = config;
    }

    public bool TryAddSelection(CardData cardData)
    {
        if (GetSelectedCount() >= config.MaxSelectedCards)
        {
            return false;
        }

        if (!config.AllowDuplicateSelection && IsCardSelected(cardData))
            return false;

        deck.AddSelectedCard(cardData);
        return true;
    }

    public bool TryRemoveSelection(CardData cardData)
    {
        if (!IsCardSelected(cardData)) return false;

        deck.RemoveSelectedCard(cardData);
        return true;
    }

    public void ClearAllSelections()
    {
        deck.ClearSelectedCards();
    }

    public bool IsCardSelected(CardData cardData) => deck?.SelectedCards.Contains(cardData) ?? false;
    public int GetSelectedCount() => deck?.SelectedCardCount ?? 0;
    public bool IsMaxReached => GetSelectedCount() >= config.MaxSelectedCards;
    public List<CardData> SelectedCards => deck?.SelectedCards.ToList() ?? new List<CardData>();
}
