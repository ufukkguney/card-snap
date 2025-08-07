using UnityEngine;
using VContainer;
using System.Threading.Tasks;

public class AIPlayer
{
    [Inject] private readonly CardSO cardSO;
    [Inject] private readonly ICardViewFactory cardViewFactory;
    
    public async Task<Card3DView> CreateRandomCardViewAsync(Transform position, GameplayConfiguration config = null)
    {
        if (cardSO == null || position == null) return null;

        var randomCardData = cardSO.GetRandomCard();
        if (randomCardData.Equals(default(CardData))) return null;

        
        var card = await cardViewFactory.CreateCardAtPosition(randomCardData, position, config);
        
        return card;
    }
}
