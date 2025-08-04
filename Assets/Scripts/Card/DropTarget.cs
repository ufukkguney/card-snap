using UnityEngine;

/// <summary>
/// Simple drop target for cards
/// </summary>
public class DropTarget : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Color highlightColor = Color.yellow;
    
    private Renderer targetRenderer;
    private Color originalColor;
    private bool isHighlighted;
    
    private void Start()
    {
        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer != null)
            originalColor = targetRenderer.material.color;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"OnTriggerEnter2D: {other.name}");
        if (other.GetComponent<CardDragHandler>()?.IsCurrentlyDragging == true)
            SetHighlight(true);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"OnTriggerExit2D: {other.name}");
        if (other.GetComponent<CardDragHandler>() != null)
            SetHighlight(false);
    }
    
    private void SetHighlight(bool highlight)
    {
        if (targetRenderer == null || isHighlighted == highlight) return;
        
        isHighlighted = highlight;
        targetRenderer.material.color = highlight ? highlightColor : originalColor;
    }
}
