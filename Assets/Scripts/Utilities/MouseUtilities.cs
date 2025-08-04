using UnityEngine;

public static class MouseUtilities
{
    public static Vector3 GetMouseWorldPosition(Camera camera, Transform referenceTransform)
    {
        if (camera == null) return referenceTransform.position;

        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = camera.WorldToScreenPoint(referenceTransform.position).z;

        return camera.ScreenToWorldPoint(mouseScreenPosition);
    }

    public static Vector3 GetValidDropPosition(Camera camera, LayerMask groundLayer, Vector3 fallbackPosition, float raycastDistance = 20f, float heightOffset = 0.1f)
    {
        if (camera == null) return fallbackPosition;

        Vector3 mouseWorldPos = GetMouseWorldPosition(camera, 0f);

        if (Physics.Raycast(mouseWorldPos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer))
        {
            return new Vector3(hit.point.x, hit.point.y + heightOffset, hit.point.z);
        }

        return fallbackPosition;
    }
    
    private static Vector3 GetMouseWorldPosition(Camera camera, float zDepth)
    {
        if (camera == null) return Vector3.zero;
        
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = zDepth;
        
        return camera.ScreenToWorldPoint(mouseScreenPosition);
    }
    
}
