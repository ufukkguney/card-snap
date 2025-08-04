using UnityEngine;
using DG.Tweening;
using System;

public class CardDragHandler : MonoBehaviour
{
    [Header("Drag Settings")]
    [SerializeField] private bool isDraggable = true;
    [SerializeField] private float dragSmoothness = 10f;
    [SerializeField] private LayerMask groundLayer = 1;
    [SerializeField] private float hoverHeight = 0.5f;
    [SerializeField] private float targetRadius = 1f;
    
    private bool isDragging;
    private Vector3 dragOffset;
    private Vector3 originalPosition;
    private Camera mainCamera;
    private Tween dragTween;
    
    public bool IsDraggable { get => isDraggable; set => isDraggable = value; }
    public bool IsCurrentlyDragging => isDragging;
    public Vector3 OriginalPosition => originalPosition;
    
    private void Start()
    {
        mainCamera = Camera.main;
        originalPosition = transform.position;
    }
    
    public void StartDragging()
    {
        if (!CanStartDrag()) return;
        
        isDragging = true;
        originalPosition = transform.position;
        dragOffset = transform.position - GetMouseWorldPosition();
        
        MoveTo(transform.position + Vector3.up * hoverHeight, 0.1f);
    }
    
    public void UpdateDragPosition()
    {
        if (!isDragging || mainCamera == null) return;
        
        Vector3 targetPosition = GetMouseWorldPosition() + dragOffset;
        MoveTo(targetPosition, 1f / dragSmoothness);
    }
    
    public void StopDragging()
    {
        if (!isDragging) return;
        
        isDragging = false;
        dragTween?.Kill();
        
        Vector3 dropPosition = GetValidDropPosition();
        MoveTo(dropPosition, 1f / dragSmoothness);
    }
    
    public void ResetToOriginalPosition()
    {
        if (!isDragging) MoveTo(originalPosition, 1f / dragSmoothness);
    }
    
    public void SetOriginalPosition(Vector3 position)
    {
        originalPosition = position;
        if (!isDragging) transform.position = position;
    }
    
    private bool CanStartDrag() => isDraggable && !isDragging;
    
    private Vector3 GetMouseWorldPosition() 
        => MouseUtilities.GetMouseWorldPosition(mainCamera, transform);
    
    private Vector3 GetValidDropPosition()
    {
        Vector3 mousePos = GetMouseWorldPosition();
        
        // Check for drop targets first (all layers)
        Collider2D[] targets = Physics2D.OverlapCircleAll(mousePos, targetRadius);
        
        // Find targets with DropTarget component
        foreach (var target in targets)
        {
            if (target.GetComponent<DropTarget>() != null)
            {
                Debug.Log($"Found drop target: {target.name} at {target.transform.position}");
                return target.transform.position;
            }
        }
        
        // Fallback to ground detection
        return MouseUtilities.GetValidDropPosition(mainCamera, groundLayer, originalPosition);
    }
    
    private void MoveTo(Vector3 targetPosition, float duration, Action onUpdate = null)
    {
        dragTween?.Kill();
        dragTween = transform.DOMove(targetPosition, duration)
            .SetEase(Ease.OutQuad);
        
        if (onUpdate != null)
            dragTween.OnUpdate(() => onUpdate());
    }
    
    private void OnDestroy() => dragTween?.Kill();
}