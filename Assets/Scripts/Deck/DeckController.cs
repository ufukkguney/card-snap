using System.Collections.Generic;
using VContainer;
using System.Threading.Tasks;
using System.Linq;

public class DeckController : IDeckController
{
    [Inject] private CardSO cardSO;
    [Inject] private EventManager eventManager;
    [Inject] private ICardViewFactory cardViewFactory;
    [Inject] private DeckSelectionManager selectionManager;
    [Inject] private CardTransformManager transformManager;
    [Inject] private DeckSelectionUIManager deckUIManager;
    [Inject] private GameplayTransitionManager gameplayTransitionManager;
    [Inject] private GameConfiguration gameConfig;
    
    private Deck deck;
    private DeckConfiguration deckConfig => gameConfig?.DeckConfig;
    private List<CardUIView> instantiatedCardViews = new List<CardUIView>();
    

    public void Initialize()
    {
        deck = new Deck(cardSO);
        deck?.InitializeDeck();

        selectionManager.Initialize(deckConfig);
        deckUIManager.Initialize(deckConfig);
        gameplayTransitionManager.Initialize(deckConfig, gameConfig.GameplayConfiguration);

        deckConfig?.StartButton.onClick.AddListener(() => gameplayTransitionManager.StartGameplay());

        eventManager.Subscribe<CardEvents.Clicked>(OnCardClicked);
        eventManager.Subscribe<GameplayEvents.ResetDeckRequested>(OnResetDeckRequested);
        eventManager.Subscribe<GameplayEvents.ReturnToDeckSelectionRequested>(OnReturnToDeckSelectionRequested);

        _ = CreateCardViewsAsync();
    }

    public void OnDestroy()
    {
        eventManager?.Unsubscribe<CardEvents.Clicked>(OnCardClicked);
        eventManager?.Unsubscribe<GameplayEvents.ResetDeckRequested>(OnResetDeckRequested);
        eventManager?.Unsubscribe<GameplayEvents.ReturnToDeckSelectionRequested>(OnReturnToDeckSelectionRequested);
        
        deckConfig?.StartButton?.onClick.RemoveAllListeners();
        
        cardViewFactory?.DestroyCardViews(instantiatedCardViews.Cast<BaseCardView>().ToList());
        instantiatedCardViews.Clear();
    }

    private async Task CreateCardViewsAsync()
    {
        if (deck?.DeckCards == null) return;

        instantiatedCardViews = await cardViewFactory.CreateCardViewsAsync(deck.DeckCards, deckConfig.OriginalCardsParent);
        transformManager.Initialize(deckConfig, instantiatedCardViews);
        deckUIManager.InitializeUI(selectionManager);
    }

    private void OnCardClicked(CardEvents.Clicked eventArgs) => ToggleCardSelection(eventArgs.CardData);
    private void OnResetDeckRequested(GameplayEvents.ResetDeckRequested evt) => ResetDeckState();
    private void OnReturnToDeckSelectionRequested(GameplayEvents.ReturnToDeckSelectionRequested evt) => ReturnToDeckSelection();

    public void ToggleCardSelection(CardData cardData)
    {
        if (selectionManager.IsCardSelected(cardData)) RemoveFromSelection(cardData);
        else AddToSelection(cardData);
    }

    private void AddToSelection(CardData cardData)
    {
        if (!selectionManager.TryAddSelection(cardData)) return;
        UpdateCardVisual(cardData, true);
    }

    private void RemoveFromSelection(CardData cardData)
    {
        if (!selectionManager.TryRemoveSelection(cardData)) return;
        UpdateCardVisual(cardData, false);
    }

    private void UpdateCardVisual(CardData cardData, bool isSelected)
    {
        if (isSelected) transformManager?.MoveCardToSelectedArea(cardData);
        else transformManager?.MoveCardToOriginalArea(cardData);
        
        deckUIManager?.UpdateStartButton(selectionManager);
    }

    public void ClearAllSelections()
    {
        selectionManager?.ClearAllSelections();
        deckUIManager?.UpdateStartButton(selectionManager);
        cardViewFactory?.DestroyCardViews(instantiatedCardViews.Cast<BaseCardView>().ToList());
        instantiatedCardViews.Clear();
    }
    
    public void ResetDeckState()
    {
        ClearAllSelections();
        deck?.ResetDeck();
        _ = CreateCardViewsAsync();
    }

    public void ReturnToDeckSelection() => gameplayTransitionManager?.ReturnToDeckSelection();

    public bool IsCardSelected(CardData cardData) => selectionManager?.IsCardSelected(cardData) ?? false;
    public int GetSelectedCount() => selectionManager?.GetSelectedCount() ?? 0;
    public List<CardUIView> GetInstantiatedCardViews() => instantiatedCardViews.ToList();
}
