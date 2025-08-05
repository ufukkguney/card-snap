using UnityEngine;
using VContainer;
using System.Threading.Tasks;

public class AIPlayer
{
    [Inject] private readonly CardSO cardSO;
    [Inject] private readonly CardViewFactory cardViewFactory;
    
    public async Task<Card3DView> CreateRandomCardViewAsync(Transform position, GameplayConfiguration config = null)
    {
        if (cardSO == null || position == null) return null;

        var randomCardData = cardSO.GetRandomCard();
        if (randomCardData.Equals(default(CardData))) return null;

        try
        {
            var card = await cardViewFactory.CreateCardAtPosition(randomCardData, position, config);
            if (card != null)
                Debug.Log($"AI created: {randomCardData.CardType} at {position.name}");
            
            return card;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AI card creation failed: {e.Message}");
            return null;
        }
    }
}
