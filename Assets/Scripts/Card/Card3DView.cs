using UnityEngine;
using TMPro;

[RequireComponent(typeof(CardDragHandler))]
public class Card3DView : BaseCardView
{
    [Header("3D UI References")]
    [SerializeField] private TextMeshPro cardNameText;
    [SerializeField] private TextMeshPro attackText;
    [SerializeField] private TextMeshPro defenseText;
    [SerializeField] private SpriteRenderer cardSprite;

    private CardDragHandler dragHandler;
    private BoxCollider2D cardCollider;

    private int originalSpriteLayer;
    private int originalTextLayer;
    private const int DRAG_LAYER_BOOST = 100;

    protected override void Start()
    {
        base.Start();
        InitializeComponents();
        StoreOriginalLayers();
    }

    private void OnMouseDown() => TryStartDrag();
    private void OnMouseDrag() => TryUpdateDrag();
    private void OnMouseUp() => TryStopDrag();


    protected override void UpdateUI()
    {
        UpdateText(cardNameText, cardData.CardType.ToString());
        UpdateText(attackText, cardData.Attack.ToString());
        UpdateText(defenseText, cardData.Defense.ToString());
    }

    public void SetDraggable(bool draggable)
    {
        if (dragHandler != null)
            dragHandler.IsDraggable = draggable;
    }

    public void ResetToOriginalPosition()
        => dragHandler?.ResetToOriginalPosition();

    public void SetOriginalPosition(Vector3 position)
        => dragHandler?.SetOriginalPosition(position);


    private void InitializeComponents()
    {
        dragHandler = GetComponent<CardDragHandler>();
        cardCollider = GetComponent<BoxCollider2D>();

        dragHandler.Initialize(this);
        ValidateComponents();
    }

    private void ValidateComponents()
    {
        if (dragHandler == null)
            Debug.LogError($"CardDragHandler missing on {gameObject.name}");

        if (cardCollider == null)
            Debug.LogWarning($"No Collider found on {gameObject.name}. Drag functionality requires a Collider.");
    }

    private void TryStartDrag()
    {
        if (CanStartDrag())
        {
            SetLayersUp();
            dragHandler.StartDragging();
        }
    }

    private void TryUpdateDrag()
    {
        if (dragHandler?.IsCurrentlyDragging == true)
            dragHandler.UpdateDragPosition();
    }

    private void TryStopDrag()
    {
        if (dragHandler?.IsCurrentlyDragging == true)
        {
            dragHandler.StopDragging();
            ResetLayersToOriginal();
        }
    }

    private bool CanStartDrag()
        => dragHandler?.IsDraggable == true && !dragHandler.IsCurrentlyDragging;

    private void UpdateText(TextMeshPro textComponent, string value)
    {
        if (textComponent != null)
            textComponent.text = value;
    }
    
    private void StoreOriginalLayers()
    {
        originalSpriteLayer = cardSprite.sortingOrder;
        originalTextLayer = cardNameText.sortingOrder;
    }

    private void SetLayersUp()
    {
        cardSprite.sortingOrder = originalSpriteLayer + DRAG_LAYER_BOOST;
        cardNameText.sortingOrder = originalTextLayer + DRAG_LAYER_BOOST;
        attackText.sortingOrder = originalTextLayer + DRAG_LAYER_BOOST;
        defenseText.sortingOrder = originalTextLayer + DRAG_LAYER_BOOST;
    }

    private void ResetLayersToOriginal()
    {
        cardSprite.sortingOrder = originalSpriteLayer;
        cardNameText.sortingOrder = originalTextLayer;
        attackText.sortingOrder = originalTextLayer;
        defenseText.sortingOrder = originalTextLayer;
    }

}
