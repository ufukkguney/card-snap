using UnityEngine;
using VContainer;
using VContainer.Unity;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

public class CardViewFactory
{
    [Inject] private readonly IObjectResolver container;
    
    private const string CARD_VIEW_KEY = "Assets/Prefabs/CardUI.prefab";
    private const string CARD_3D_VIEW_KEY = "Assets/Prefabs/CardWorldPlace.prefab";
    
    private GameObject cachedCardViewPrefab;
    private GameObject cachedCard3DViewPrefab;

    public async Task<CardUIView> CreateCardViewAsync(CardData cardData, Transform parent = null)
    {
        var prefab = await GetCardViewPrefabAsync();
        return CreateInstance<CardUIView>(prefab, cardData, parent);
    }

    public async Task<Card3DView> CreateCard3DViewAsync(CardData cardData, Transform parent = null)
    {
        var prefab = await GetCard3DViewPrefabAsync();
        return CreateInstance<Card3DView>(prefab, cardData, parent);
    }

    public async Task<List<CardUIView>> CreateCardViewsAsync(List<CardData> cardDataList, Transform parent = null)
    {
        var cardViews = new List<CardUIView>();
        foreach (var cardData in cardDataList)
        {
            var cardView = await CreateCardViewAsync(cardData, parent);
            if (cardView != null) 
            {
                cardView.Initialize();
                cardViews.Add(cardView);
            }
        }
        return cardViews;
    }

    public async Task<List<Card3DView>> CreateCard3DViewsAsync(List<CardData> cardDataList, Transform parent = null)
    {
        var card3DViews = new List<Card3DView>();
        foreach (var cardData in cardDataList)
        {
            var card3DView = await CreateCard3DViewAsync(cardData, parent);
            if (card3DView != null) 
            {
                card3DViews.Add(card3DView);
            }
        }
        return card3DViews;
    }

    public async Task<List<Card3DView>> CreateCard3DViewsAtPositionsAsync(
        List<CardData> cardDataList, 
        GameplayConfiguration gameplayConfig)
    {
        if (gameplayConfig == null || !gameplayConfig.ArePositionsValid())
        {
            Debug.LogError("Invalid gameplay configuration or positions");
            return new List<Card3DView>();
        }

        var card3DViews = new List<Card3DView>();
        int maxCards = Mathf.Min(cardDataList.Count, gameplayConfig.MaxGameplayCards);

        var creationTasks = new List<Task<Card3DView>>();
        
        for (int i = 0; i < maxCards; i++)
        {
            var cardData = cardDataList[i];
            var targetPosition = gameplayConfig.GetCardPosition(i);

            if (targetPosition == null)
            {
                Debug.LogWarning($"Position {i} is null, skipping card");
                continue;
            }

            creationTasks.Add(CreateSingleCard3DViewAsync(cardData, targetPosition, gameplayConfig));
        }

        var createdCards = await Task.WhenAll(creationTasks);

        foreach (var card in createdCards)
        {
            if (card != null)
            {
                card3DViews.Add(card);
            }
        }

        Debug.Log($"Created {card3DViews.Count} cards simultaneously at designated positions");
        return card3DViews;
    }

    private async Task<Card3DView> CreateSingleCard3DViewAsync(
        CardData cardData, 
        Transform targetPosition, 
        GameplayConfiguration gameplayConfig)
    {
        try
        {
            var card3DView = await CreateCard3DViewAsync(cardData);
            
            if (card3DView != null)
            {
                PlaceCardAtPosition(card3DView, targetPosition, gameplayConfig);
                return card3DView;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create card at position {targetPosition.name}: {e.Message}");
        }

        return null;
    }

    private void PlaceCardAtPosition(Card3DView card3DView, Transform targetPosition, GameplayConfiguration gameplayConfig)
    {
        if (card3DView == null || targetPosition == null) return;

        card3DView.transform.position = targetPosition.position;
        card3DView.transform.rotation = targetPosition.rotation;
        
        if (targetPosition.parent != null)
        {
            card3DView.transform.SetParent(targetPosition.parent);
        }

        if (gameplayConfig.cardPlacementEffect != null)
        {
            Object.Instantiate(gameplayConfig.cardPlacementEffect, targetPosition.position, targetPosition.rotation);
        }

        if (gameplayConfig.cardPlacementSound != null)
        {
            var audioSource = card3DView.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.PlayOneShot(gameplayConfig.cardPlacementSound);
            }
        }

        Debug.Log($"Card placed at position: {targetPosition.name} ({targetPosition.position})");
    }

    public CardUIView CreateCardView(CardData cardData, Transform parent = null)
        => CreateCardViewAsync(cardData, parent).GetAwaiter().GetResult();

    public List<CardUIView> CreateCardViews(List<CardData> cardDataList, Transform parent = null)
        => CreateCardViewsAsync(cardDataList, parent).GetAwaiter().GetResult();

    public void DestroyCardView(BaseCardView cardView)
        => Object.DestroyImmediate(cardView?.gameObject);

    public void DestroyCardViews(List<BaseCardView> cardViews)
        => cardViews?.ForEach(DestroyCardView);

    public void Cleanup()
    {
        if (cachedCardViewPrefab != null)
        {
            Addressables.Release(cachedCardViewPrefab);
            cachedCardViewPrefab = null;
        }
        
        if (cachedCard3DViewPrefab != null)
        {
            Addressables.Release(cachedCard3DViewPrefab);
            cachedCard3DViewPrefab = null;
        }
    }

    private T CreateInstance<T>(GameObject prefab, CardData cardData, Transform parent) where T : BaseCardView
    {
        if (prefab == null) return null;
        
        var instance = container.Instantiate(prefab, parent);
        var component = instance.GetComponent<T>();
        component?.SetCardData(cardData);
        return component;
    }

    private async Task<GameObject> GetCardViewPrefabAsync()
        => cachedCardViewPrefab ??= await LoadGameObjectAsync(CARD_VIEW_KEY);

    private async Task<GameObject> GetCard3DViewPrefabAsync()
        => cachedCard3DViewPrefab ??= await LoadGameObjectAsync(CARD_3D_VIEW_KEY);

    private async Task<GameObject> LoadGameObjectAsync(string key)
    {
        try
        {
            return await Addressables.LoadAssetAsync<GameObject>(key).Task;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load GameObject with key '{key}': {e.Message}");
            return null;
        }
    }
}
