using UnityEngine;

public static class Ray
{
    public static bool Cast(Vector2 start, Vector2 end, out Hit hit)
    {
        var dir = (end - start).normalized;

        Debug.Log(dir);

        foreach (var collider in CircleCollider.Colliders)
        {
            var colPos = collider.Key.transform.position;
            
            if (Vector2.Dot(dir, colPos) > 0)
            {
                var colDir = new Vector2(start.x - colPos.x, start.y - colPos.y);
                var d = Vector3.Cross(colDir, dir).magnitude / dir.magnitude;
                
                if (d <= collider.Value.Radius)
                {
                    hit = new Hit(Vector2.zero, collider.Key);
                    return true;
                }
            }
        }

        hit = new Hit();

        return false;
    }
}

public struct Hit
{
    public readonly Vector2 point;

    public readonly GameObject go;

    public Hit(Vector2 point = new Vector2(), GameObject go = null)
    {
        this.point = point;
        this.go = go;
    }
}
