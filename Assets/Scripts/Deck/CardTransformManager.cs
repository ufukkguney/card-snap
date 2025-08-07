using System.Collections.Generic;
using System.Linq;

public class CardTransformManager
{
    private DeckConfiguration config;
    private List<CardUIView> cardViews;

    public void Initialize(DeckConfiguration config, List<CardUIView> cardViews)
    {
        this.config = config;
        this.cardViews = cardViews;
    }

    public void MoveCardToSelectedArea(CardData cardData)
    {
        var cardView = GetCardViewByData(cardData);
        if (cardView != null && config.SelectedCardsParent != null)
        {
            cardView.transform.SetParent(config.SelectedCardsParent);
        }
    }

    public void MoveCardToOriginalArea(CardData cardData)
    {
        var cardView = GetCardViewByData(cardData);
        if (cardView != null && config.OriginalCardsParent != null)
        {
            cardView.transform.SetParent(config.OriginalCardsParent);
        }
    }

    private CardUIView GetCardViewByData(CardData cardData)
        => cardViews.FirstOrDefault(cv => cv?.Data.Equals(cardData) == true);
}
