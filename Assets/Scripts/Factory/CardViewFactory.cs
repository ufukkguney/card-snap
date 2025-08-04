using UnityEngine;
using VContainer;
using VContainer.Unity;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using System.Linq;

public class CardViewFactory
{
    [Inject] private readonly IObjectResolver container;
    
    private const string CARD_UI_KEY = "Assets/Prefabs/CardUI.prefab";
    private const string CARD_3D_KEY = "Assets/Prefabs/CardWorldPlace.prefab";
    
    private GameObject cachedCardUI, cachedCard3D;

    public async Task<List<CardUIView>> CreateCardViewsAsync(List<CardData> cardDataList, Transform parent = null)
    {
        var prefab = await LoadAsset(CARD_UI_KEY);
        return cardDataList.Select(data => CreateInstance<CardUIView>(prefab, data, parent))
                          .Where(card => card != null)
                          .ToList();
    }

    public async Task<List<Card3DView>> CreateCard3DViewsAtPositionsAsync(
        List<CardData> cardDataList, 
        GameplayConfiguration gameplayConfig)
    {
        if (!IsValidGameplayConfig(gameplayConfig)) 
            return new List<Card3DView>();

        var maxCards = Mathf.Min(cardDataList.Count, gameplayConfig.MaxGameplayCards);
        var tasks = Enumerable.Range(0, maxCards)
            .Select(i => CreateCardAtPosition(cardDataList[i], gameplayConfig.GetCardPosition(i), gameplayConfig))
            .Where(task => task != null);

        var results = await Task.WhenAll(tasks);
        var successfulCards = results.Where(card => card != null).ToList();
        
        Debug.Log($"Created {successfulCards.Count}/{cardDataList.Count} cards at designated positions");
        return successfulCards;
    }
    
    private async Task<Card3DView> CreateCard3DViewAsync(CardData cardData, Transform parent = null) 
        => await CreateCard<Card3DView>(CARD_3D_KEY, cardData, parent);

    public void DestroyCardView(BaseCardView cardView)
        => Object.DestroyImmediate(cardView?.gameObject);

    public void DestroyCardViews(List<BaseCardView> cardViews) 
        => cardViews?.ForEach(DestroyCardView);

    public void Cleanup()
    {
        ReleaseAsset(ref cachedCardUI);
        ReleaseAsset(ref cachedCard3D);
    }

    private async Task<T> CreateCard<T>(string assetKey, CardData cardData, Transform parent) where T : BaseCardView
    {
        var prefab = await LoadAsset(assetKey);
        return CreateInstance<T>(prefab, cardData, parent);
    }

    private async Task<Card3DView> CreateCardAtPosition(CardData cardData, Transform position, GameplayConfiguration config)
    {
        if (position == null)
        {
            Debug.LogWarning($"Position is null for card {cardData.CardType}, skipping");
            return null;
        }

        try
        {
            var card = await CreateCard3DViewAsync(cardData);
            if (card != null) PlaceCardAtPosition(card, position, config);
            return card;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create card {cardData.CardType} at position {position.name}: {e.Message}");
            return null;
        }
    }

    private T CreateInstance<T>(GameObject prefab, CardData cardData, Transform parent) where T : BaseCardView
    {
        if (prefab == null) return null;
        
        var instance = container.Instantiate(prefab, parent);
        var component = instance.GetComponent<T>();
        
        if (component != null)
        {
            component.SetCardData(cardData);
            
            if (component is CardUIView uiCard)
                uiCard.Initialize();
        }
        
        return component;
    }

    private void PlaceCardAtPosition(Card3DView card, Transform position, GameplayConfiguration config)
    {
        card.transform.position = position.position;
        card.transform.rotation = position.rotation;
        
        if (position.parent != null)
            card.transform.SetParent(position.parent);

        if (config.cardPlacementEffect != null)
            Object.Instantiate(config.cardPlacementEffect, position.position, position.rotation);

        if (config.cardPlacementSound != null)
            card.GetComponent<AudioSource>()?.PlayOneShot(config.cardPlacementSound);
    }

    private async Task<GameObject> LoadAsset(string key)
    {
        try
        {
            if (key == CARD_UI_KEY)
                return cachedCardUI ??= await Addressables.LoadAssetAsync<GameObject>(key).Task;
            else
                return cachedCard3D ??= await Addressables.LoadAssetAsync<GameObject>(key).Task;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load asset '{key}': {e.Message}");
            return null;
        }
    }

    private void ReleaseAsset(ref GameObject asset)
    {
        if (asset != null)
        {
            Addressables.Release(asset);
            asset = null;
        }
    }

    private bool IsValidGameplayConfig(GameplayConfiguration config)
    {
        if (config?.ArePositionsValid() != true)
        {
            Debug.LogError("Invalid gameplay configuration or positions");
            return false;
        }
        return true;
    }

}
