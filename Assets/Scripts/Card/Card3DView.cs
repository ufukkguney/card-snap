using UnityEngine;
using TMPro;

[RequireComponent(typeof(CardDragHandler))]
public class Card3DView : BaseCardView
{
    [Header("3D References")]
    [SerializeField] private TextMeshPro cardNameText;
    [SerializeField] private TextMeshPro attackText;
    [SerializeField] private TextMeshPro defenseText;
    [SerializeField] private SpriteRenderer cardSprite;
    
    [Header("Components")]
    [SerializeField] private CardDragHandler dragHandler;
    
    private BoxCollider2D cardCollider;
    
    public System.Action<Card3DView> CardClicked;
    public System.Action<Card3DView> CardDragStarted;
    public System.Action<Card3DView> CardDragEnded;
    public System.Action<Card3DView, Vector3> CardDragging;
    
    protected override void Start()
    {
        base.Start();
        cardCollider = GetComponent<BoxCollider2D>();
        
        if (dragHandler == null)
            dragHandler = GetComponent<CardDragHandler>();
        
        if (cardCollider == null)
        {
            Debug.LogWarning($"No Collider found on {gameObject.name}. Drag functionality requires a Collider.");
        }
    }
    
    private void OnMouseDown()
    {
        if (dragHandler != null && dragHandler.IsDraggable && !dragHandler.IsCurrentlyDragging)
        {
            dragHandler.StartDragging();
        }
        else
        {
            OnCardClicked();
        }
    }
    
    private void OnMouseDrag()
    {
        if (dragHandler != null && dragHandler.IsCurrentlyDragging)
        {
            dragHandler.UpdateDragPosition();
        }
    }
    
    private void OnMouseUp()
    {
        if (dragHandler != null && dragHandler.IsCurrentlyDragging)
        {
            dragHandler.StopDragging();
        }
    }
    
    protected override void UpdateUI()
    {
        if (cardNameText != null)
            cardNameText.text = cardData.CardName;
            
        if (attackText != null)
            attackText.text = cardData.Attack.ToString();
            
        if (defenseText != null)
            defenseText.text = cardData.Defense.ToString();
    }
    
    protected override void UpdateVisuals()
    {
        if (cardSprite != null)
        {
            cardSprite.color = GetCurrentColor();
        }
    }
    
    public override void HighlightCard(bool highlight)
    {
        if (cardSprite != null)
        {
            cardSprite.color = highlight ? Color.green : GetCurrentColor();
        }
    }
    
    protected override void NotifyCardClicked()
    {
        CardClicked?.Invoke(this);
    }
    
    public void SetDraggable(bool draggable)
    {
        if (dragHandler != null)
            dragHandler.IsDraggable = draggable;
    }
    
    public bool IsDragging()
    {
        return dragHandler != null && dragHandler.IsCurrentlyDragging;
    }
    
    public void ResetToOriginalPosition()
    {
        if (dragHandler != null)
            dragHandler.ResetToOriginalPosition();
    }
    
    public void SetOriginalPosition(Vector3 position)
    {
        if (dragHandler != null)
            dragHandler.SetOriginalPosition(position);
    }
    
    private void OnDestroy()
    {
       
    }
}
