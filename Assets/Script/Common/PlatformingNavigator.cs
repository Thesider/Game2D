using UnityEngine;

public static class PlatformingNavigator
{

    private static LayerMask? cachedGroundMask;
    private static LayerMask? cachedObstacleMask;

    public static LayerMask GroundMask
    {
        get
        {
            if (!cachedGroundMask.HasValue)
            {
                int mask = LayerMask.GetMask("Ground", "Platforms", "Default");
                if (mask == 0)
                {
                    mask = Physics2D.DefaultRaycastLayers;
                }
                cachedGroundMask = mask;
            }

            return cachedGroundMask.Value;
        }
    }

    public static LayerMask ObstacleMask
    {
        get
        {
            if (!cachedObstacleMask.HasValue)
            {
                cachedObstacleMask = Physics2D.DefaultRaycastLayers;
            }

            return cachedObstacleMask.Value;
        }
    }

    public static bool DetectObstacle(Vector2 origin, Vector2 direction, float distance, LayerMask mask, out RaycastHit2D hit)
    {
        if (mask == 0)
        {
            mask = ObstacleMask;
        }

        hit = Physics2D.Raycast(origin, direction.normalized, Mathf.Max(0.01f, distance), mask);
        return hit.collider != null;
    }

    public static bool IsGapAhead(Vector2 origin, Vector2 direction, float horizontalDistance, float rayDepth, LayerMask groundMask)
    {
        if (groundMask == 0)
        {
            groundMask = GroundMask;
        }

        Vector2 probePoint = origin + direction.normalized * Mathf.Max(0.05f, horizontalDistance);
        RaycastHit2D hit = Physics2D.Raycast(probePoint, Vector2.down, Mathf.Max(0.05f, rayDepth), groundMask);
        return hit.collider == null;
    }

    public static bool IsGrounded(Vector2 origin, float checkDistance, LayerMask groundMask)
    {
        if (groundMask == 0)
        {
            groundMask = GroundMask;
        }

        RaycastHit2D hit = Physics2D.Raycast(origin + Vector2.up * 0.05f, Vector2.down, Mathf.Max(0.05f, checkDistance), groundMask);
        return hit.collider != null;
    }

    public static bool HasHeadroom(Vector2 origin, float height, LayerMask obstacleMask)
    {
        if (obstacleMask == 0)
        {
            obstacleMask = ObstacleMask;
        }

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, Mathf.Max(0.05f, height), obstacleMask);
        return hit.collider == null;
    }

    public static bool ShouldJumpForHeight(Vector3 selfPosition, Vector3 targetPosition, float heightThreshold)
    {
        return targetPosition.y - selfPosition.y > heightThreshold;
    }

    public static Vector2 GroundProbeOrigin(Transform transform, float heightOffset = 0.1f)
    {
        if (transform == null)
        {
            return Vector2.zero;
        }

        return (Vector2)transform.position + Vector2.up * heightOffset;
    }
}