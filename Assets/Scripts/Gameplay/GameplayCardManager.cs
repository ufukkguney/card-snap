using VContainer;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GameplayCardManager
{
    [Inject] private ICardViewFactory cardViewFactory;
    private GameplayConfiguration gameplayConfig;
    [Inject] private GameplayUIController uiController;
    [Inject] private GameplayStateManager stateManager;

    public void Initialize(GameplayConfiguration config)
    {
        gameplayConfig = config;
    }
    public async Task CreateCards(List<CardData> selectedCards)
    {
        var card3DViews = await cardViewFactory.CreateCard3DViewsAtPositionsAsync(
            selectedCards,
            gameplayConfig
        );

        uiController?.StartTimer();
    }

    public void SetCurrentPlayerCard(Card3DView card) => stateManager.SetCurrentPlayerCard(card);
}