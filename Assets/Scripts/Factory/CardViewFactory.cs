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
    private const string CARD_3D_VIEW_KEY = "Card3DView";
    
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
