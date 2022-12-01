using UnityEngine;

public class WorldBounds : MonoBehaviour
{
    private static Vector2 _bounds;

    void Awake() => _bounds = transform.localScale * 0.5f;

    public static bool CheckInBounds(Vector3 nextPosition, float radius)
    {
        if (nextPosition.x + radius < _bounds.x && nextPosition.x - radius > -_bounds.x &&
            nextPosition.y + radius < _bounds.y && nextPosition.y - radius > -_bounds.y)
            return false;
        else
            return true;
    }

    public static void KeepInBounds(ref Vector3 nextPosition, float radius)
    {
        nextPosition.x = Mathf.Clamp(nextPosition.x, -_bounds.x + radius, _bounds.x - radius); 
        nextPosition.y = Mathf.Clamp(nextPosition.y, -_bounds.y + radius, _bounds.y - radius);
    }
}
