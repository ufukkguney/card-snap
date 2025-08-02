using UnityEngine;
using DG.Tweening;

public class CardDragHandler : MonoBehaviour
{
    [Header("Drag Settings")]
    [SerializeField] private bool isDraggable = true;
    [SerializeField] private float dragSmoothness = 10f;
    [SerializeField] private LayerMask groundLayer = 1;
    [SerializeField] private float hoverHeight = 0.5f;
    
    private bool isDragging = false;
    private Vector3 dragOffset;
    private Vector3 originalPosition;
    private Camera mainCamera;
    private Tween dragTween;
    
    public System.Action<Transform> OnDragStarted;
    public System.Action<Transform> OnDragEnded;
    public System.Action<Transform, Vector3> OnDragging;
    
    public bool IsDraggable 
    { 
        get => isDraggable; 
        set => isDraggable = value; 
    }
    
    public bool IsCurrentlyDragging => isDragging;
    public Vector3 OriginalPosition => originalPosition;
    
    private void Start()
    {
        mainCamera = Camera.main;
        originalPosition = transform.position;
    }
    
    public void StartDragging()
    {
        if (!isDraggable || isDragging) return;
        
        isDragging = true;
        originalPosition = transform.position;

        Vector3 mousePosition = GetMouseWorldPosition();
        dragOffset = transform.position - mousePosition;
        
        transform.position += Vector3.up * hoverHeight;
        
        OnDragStarted?.Invoke(transform);
    }
    
    public void UpdateDragPosition()
    {
        if (!isDragging || mainCamera == null) return;
        
        Vector3 targetPosition = GetMouseWorldPosition() + dragOffset;
        
        dragTween?.Kill();
        
        dragTween = transform.DOMove(targetPosition, 1f / dragSmoothness)
            .SetEase(Ease.OutQuad)
            .OnUpdate(() => {
                OnDragging?.Invoke(transform, transform.position);
            });
    }
    
    public void StopDragging()
    {
        if (!isDragging) return;
        isDragging = false;

        dragTween?.Kill();
        Vector3 dropPosition = GetValidDropPosition();
        MoveToPosition(dropPosition);
        
        OnDragEnded?.Invoke(transform);
    }
    
    public void ResetToOriginalPosition()
    {
        if (!isDragging)
        {
            MoveToPosition(originalPosition);
        }
    }
    
    public void SetOriginalPosition(Vector3 position)
    {
        originalPosition = position;
        if (!isDragging)
        {
            transform.position = position;
        }
    }
    
    private Vector3 GetMouseWorldPosition()
    {
        return MouseUtilities.GetMouseWorldPosition(mainCamera, transform);
    }
    
    private Vector3 GetValidDropPosition()
    {
        return MouseUtilities.GetValidDropPosition(mainCamera, groundLayer, originalPosition);
    }
    
    private void MoveToPosition(Vector3 targetPosition)
    {
        float duration = 1f / dragSmoothness;
        transform.DOMove(targetPosition, duration)
            .SetEase(Ease.OutQuad);
    }
    
    private void OnDestroy()
    {
        dragTween?.Kill();
    }
}