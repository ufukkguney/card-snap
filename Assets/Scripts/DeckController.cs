using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using System.Threading.Tasks;
using System.Linq;

/// <summary>
/// Deck işlemlerini koordine eden ana controller
/// Sadece farklı manager'ları koordine eder, iş mantığı yapmaz
/// </summary>
public class DeckController : MonoBehaviour
{
    [Inject] private CardSO cardSO;
    [Inject] private EventManager eventManager;
    [Inject] private CardViewFactory cardViewFactory;

    [Header("Deck Configuration")]
    [SerializeField] private DeckConfiguration deckConfig;

    // Managers - tek sorumluluk prensibine göre
    private SelectionManager selectionManager;
    private CardTransformManager transformManager;
    private SelectionUIManager uiManager;
    
    private Deck deck;
    private List<CardUIView> instantiatedCardViews = new List<CardUIView>();

    public void Initialize()
    {
        // 1. Deck'i başlat
        deck = new Deck(cardSO, eventManager);
        deck?.InitializeDeck();
        
        // 2. Manager'ları oluştur
        selectionManager = new SelectionManager(deck, eventManager, deckConfig);
        uiManager = new SelectionUIManager(deckConfig);
        
        // 3. Event'leri dinle
        eventManager.Subscribe<CardEvents.Clicked>(OnCardClicked);
        
        // 4. Card view'ları oluştur
        _ = CreateCardViewsAsync();
        
        Debug.Log("DeckController initialized successfully");
    }

    private void OnDestroy()
    {
        eventManager?.Unsubscribe<CardEvents.Clicked>(OnCardClicked);
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
            
            // Transform manager'ı oluştur (card view'lar hazır olduktan sonra)
            transformManager = new CardTransformManager(deckConfig, instantiatedCardViews);
            
            // UI'ı başlat
            transformManager.UpdateAllCardSelections(selectionManager);
            uiManager.InitializeUI(selectionManager);
            
            Debug.Log($"Created {instantiatedCardViews.Count} card views");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create card views: {e.Message}");
        }
    }

    // Event handler - sadece koordinasyon
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
        // 1. Selection manager ile iş mantığını kontrol et
        if (!selectionManager.TryAddSelection(cardData)) return;

        // 2. UI güncellemelerini koordine et
        transformManager?.MoveCardToSelectedArea(cardData);
        transformManager?.UpdateCardSelection(cardData, true);
        uiManager?.UpdateStartButton(selectionManager);
    }

    private void RemoveFromSelection(CardData cardData)
    {
        // 1. Selection manager ile iş mantığını kontrol et
        if (!selectionManager.TryRemoveSelection(cardData)) return;

        // 2. UI güncellemelerini koordine et
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

    // Public API - sadece delegation
    public bool IsCardSelected(CardData cardData) => selectionManager?.IsCardSelected(cardData) ?? false;
    public int GetSelectedCount() => selectionManager?.GetSelectedCount() ?? 0;
    public List<CardUIView> GetInstantiatedCardViews() => instantiatedCardViews.ToList();
}
