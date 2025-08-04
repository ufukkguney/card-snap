using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using System.Threading.Tasks;
using System.Linq;

public class DeckController : MonoBehaviour
{
    [Inject] private CardSO cardSO;
    [Inject] private EventManager eventManager;
    [Inject] private CardViewFactory cardViewFactory;

    [Header("Deck Configuration")]
    [SerializeField] private DeckConfiguration deckConfig;

    private SelectionManager selectionManager;
    private CardTransformManager transformManager;
    private SelectionUIManager uiManager;
    private GameplayTransitionManager gameplayTransitionManager;
    
    private Deck deck;
    private List<CardUIView> instantiatedCardViews = new List<CardUIView>();

    public void Initialize()
    {
        deck = new Deck(cardSO);
        deck?.InitializeDeck();

        selectionManager = new SelectionManager(deck, deckConfig);
        uiManager = new SelectionUIManager(deckConfig);
        gameplayTransitionManager = new GameplayTransitionManager(deckConfig, selectionManager, cardViewFactory);
        
        deckConfig?.StartButton.onClick.AddListener(() => gameplayTransitionManager.StartGameplay());
        
        eventManager.Subscribe<CardEvents.Clicked>(OnCardClicked);
        _ = CreateCardViewsAsync();

        Debug.Log("DeckController initialized successfully");
    }

    private void OnDestroy()
    {
        eventManager?.Unsubscribe<CardEvents.Clicked>(OnCardClicked);
        
        if (deckConfig?.StartButton != null)
        {
            deckConfig.StartButton.onClick.RemoveAllListeners();
        }
        
        cardViewFactory?.DestroyCardViews(instantiatedCardViews.Cast<BaseCardView>().ToList());
        cardViewFactory?.Cleanup();
        instantiatedCardViews.Clear();
    }

    private async Task CreateCardViewsAsync()
    {
        if (deck?.DeckCards == null) return;

        try
        {
            instantiatedCardViews = await cardViewFactory.CreateCardViewsAsync(deck.DeckCards, deckConfig.originalCardsParent);

            transformManager = new CardTransformManager(deckConfig, instantiatedCardViews);

            transformManager.UpdateAllCardSelections(selectionManager);
            uiManager.InitializeUI(selectionManager);

            Debug.Log($"Created {instantiatedCardViews.Count} card views");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create card views: {e.Message}");
        }
    }

    private void OnCardClicked(CardEvents.Clicked eventArgs)
        => ToggleCardSelection(eventArgs.CardData);

    public void ToggleCardSelection(CardData cardData)
    {
        if (selectionManager.IsCardSelected(cardData))
            RemoveFromSelection(cardData);
        else
            AddToSelection(cardData);
    }

    private void AddToSelection(CardData cardData)
    {
        if (!selectionManager.TryAddSelection(cardData)) return;

        transformManager?.MoveCardToSelectedArea(cardData);
        transformManager?.UpdateCardSelection(cardData, true);
        uiManager?.UpdateStartButton(selectionManager);
    }

    private void RemoveFromSelection(CardData cardData)
    {
        if (!selectionManager.TryRemoveSelection(cardData)) return;

        transformManager?.MoveCardToOriginalArea(cardData);
        transformManager?.UpdateCardSelection(cardData, false);
        uiManager?.UpdateStartButton(selectionManager);
    }

    public void ClearAllSelections()
    {
        selectionManager?.ClearAllSelections();
        transformManager?.UpdateAllCardSelections(selectionManager);
        uiManager?.UpdateStartButton(selectionManager);
    }

    public bool IsCardSelected(CardData cardData) => selectionManager?.IsCardSelected(cardData) ?? false;
    public int GetSelectedCount() => selectionManager?.GetSelectedCount() ?? 0;
    public List<CardUIView> GetInstantiatedCardViews() => instantiatedCardViews.ToList();
}
