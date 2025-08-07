using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface ICardViewFactory
{
    Task<List<CardUIView>> CreateCardViewsAsync(List<CardData> cardDataList, Transform parent = null);
    Task<List<Card3DView>> CreateCard3DViewsAtPositionsAsync(List<CardData> cardDataList, GameplayConfiguration gameplayConfig);
    Task<Card3DView> CreateCardAtPosition(CardData cardData, Transform position, GameplayConfiguration config);
    void DestroyCardView(BaseCardView cardView);
    void DestroyCardViews(List<BaseCardView> cardViews);
}