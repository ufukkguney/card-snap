using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardTransformManager
{
    private readonly DeckConfiguration config;
    private readonly List<CardUIView> cardViews;

    public CardTransformManager(DeckConfiguration config, List<CardUIView> cardViews)
    {
        this.config = config;
        this.cardViews = cardViews;
    }

    public void MoveCardToSelectedArea(CardData cardData)
    {
        var cardView = GetCardViewByData(cardData);
        if (cardView != null && config.selectedCardsParent != null)
        {
            cardView.transform.SetParent(config.selectedCardsParent);
            Debug.Log($"Moved card {cardData.CardType} to selected area");
        }
    }

    public void MoveCardToOriginalArea(CardData cardData)
    {
        var cardView = GetCardViewByData(cardData);
        if (cardView != null && config.originalCardsParent != null)
        {
            cardView.transform.SetParent(config.originalCardsParent);
            Debug.Log($"Moved card {cardData.CardType} back to original area");
        }
    }

    public void UpdateCardSelection(CardData cardData, bool isSelected)
    {
        cardViews.Where(cv => cv?.Data.Equals(cardData) == true)
                .ToList()
                .ForEach(cv => cv.IsSelected = isSelected);
    }

    public void UpdateAllCardSelections(SelectionManager selectionManager)
    {
        cardViews.Where(cv => cv != null)
                .ToList()
                .ForEach(cv => cv.IsSelected = selectionManager.IsCardSelected(cv.Data));
    }

    private CardUIView GetCardViewByData(CardData cardData)
        => cardViews.FirstOrDefault(cv => cv?.Data.Equals(cardData) == true);
}
