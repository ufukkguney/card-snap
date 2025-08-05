using UnityEngine;
using VContainer;
public class DropTarget : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private Color blockedColor = Color.red;
    
    private Renderer targetRenderer;
    private Color originalColor;
    private bool isHighlighted;
    private Card3DView currentCard;
    
    [Inject] private GamePlayController gameManager;
    
    public bool HasCard => currentCard != null;
    public Card3DView CurrentCard => currentCard;
    
    private void Start()
    {
        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer != null)
            originalColor = targetRenderer.material.color;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        CardDragHandler dragHandler = other.GetComponent<CardDragHandler>();
        
        if (dragHandler?.IsCurrentlyDragging == true)
        {
            bool canAccept = CanAcceptCard();
            SetHighlight(true, canAccept);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        CardDragHandler dragHandler = other.GetComponent<CardDragHandler>();
        if (other.GetComponent<CardDragHandler>() != null) 
            SetHighlight(false);

        if (dragHandler.card3DView != currentCard) return;

        RemoveCard();

        Debug.Log($"OnTriggerExit2D: {other.name}");
    }
    
    public bool CanAcceptCard()
    {
        return !HasCard;
    }
    
    public void PlaceCard(Card3DView cardView)
    {
        if (HasCard || cardView == null) return;
        
        currentCard = cardView;
        Debug.Log($"Card {cardView.name} placed on {gameObject.name}");
        
        gameManager?.OnCardPlacedOnTarget(this, cardView);
    }
    
    public void RemoveCard()
    {
        if (!HasCard) return;
        
        var removedCard = currentCard;
        currentCard = null;
        Debug.Log($"Card {removedCard.name} removed from {gameObject.name}");
        
        gameManager?.OnCardRemovedFromTarget(this, removedCard);
    }
    
    public void ForceRemoveCard()
    {
        if (HasCard)
        {
            var removedCard = currentCard;
            currentCard = null;
            Debug.Log($"Card {removedCard.name} force removed from {gameObject.name}");
        }
    }
    
    private void SetHighlight(bool highlight, bool canAccept = true)
    {
        if (targetRenderer == null || isHighlighted == highlight) return;
        
        isHighlighted = highlight;
        
        if (highlight)
        {
            Color targetColor = canAccept ? highlightColor : blockedColor;
            targetRenderer.material.color = targetColor;
        }
        else
        {
            targetRenderer.material.color = originalColor;
        }
    }
}
