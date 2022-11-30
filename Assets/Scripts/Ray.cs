using UnityEngine;

public static class Ray
{
    public static bool Cast(GameObject caster, Vector3 origin, Vector3 dir, out GameObject hit)
    {
        foreach (var collider in CircleCollider.Colliders)
        {
            if (collider.Key == caster)
                continue;

            var circle = collider.Key.transform.position;
            var otc = circle - origin;

            if (Vector3.Dot(otc, dir) > 0)
            {
                var v = Vector3.Cross(otc, dir);
                var d = v.magnitude / dir.magnitude;
                
                if (d <= collider.Value.Radius)
                {
                    hit = collider.Key;
                    return true;
                }
            }
        }

        hit = null;
        return false;
    }
}
