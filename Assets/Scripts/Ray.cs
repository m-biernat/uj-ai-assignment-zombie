using UnityEngine;

public static class Ray
{
    public static bool Cast(GameObject caster, Vector3 origin, Vector3 dir, out GameObject hit)
    {
        var result = false;
        hit = null;    

        var minDist = Mathf.Infinity;

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
                
                var dist = otc.sqrMagnitude;

                if (d <= collider.Value.Radius && dist < minDist)
                {
                    minDist = dist;
                    hit = collider.Key;
                    result = true;
                }
            }
        }

        return result;
    }
}
