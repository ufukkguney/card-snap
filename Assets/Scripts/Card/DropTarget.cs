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
        if (targetRenderer != null) originalColor = targetRenderer.material.color;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var dragHandler = other.GetComponent<CardDragHandler>();
        if (dragHandler?.IsCurrentlyDragging == true) SetHighlight(true, CanAcceptCard());
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        var dragHandler = other.GetComponent<CardDragHandler>();
        if (dragHandler != null) SetHighlight(false);
        if (dragHandler?.Card3DView == currentCard) RemoveCard();
    }
    
    public bool CanAcceptCard() => !HasCard;
    
    public void PlaceCard(Card3DView cardView)
    {
        if (HasCard || cardView == null) return;
        currentCard = cardView;
        gameManager?.OnCardPlacedOnTarget(this, cardView);
    }
    
    public void RemoveCard()
    {
        if (!HasCard) return;
        var removedCard = currentCard;
        currentCard = null;
        gameManager?.OnCardRemovedFromTarget(this, removedCard);
    }
    
    
    private void SetHighlight(bool highlight, bool canAccept = true)
    {
        if (targetRenderer == null || isHighlighted == highlight) return;
        isHighlighted = highlight;
        targetRenderer.material.color = highlight ? (canAccept ? highlightColor : blockedColor) : originalColor;
    }
}
