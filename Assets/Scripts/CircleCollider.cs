using UnityEngine;
using System.Collections.Generic;

public class CircleCollider : MonoBehaviour
{
    [field: SerializeField]
    public float Radius { get; private set; }

    public static Dictionary<GameObject, CircleCollider> Colliders {get; private set;}

    void Awake() 
    {
        Radius = transform.localScale.x * 0.5f;

        if (Colliders == null)
            Colliders = new Dictionary<GameObject, CircleCollider>();
        
        Colliders.Add(gameObject, this);
    }

    void OnDestroy() => Colliders.Remove(gameObject);

    public static Vector3 ResolveCollision(GameObject go, Vector3 nextPosition)
    {
        var colliderA = Colliders[go];
        var colliderAPos = colliderA.transform.position;

        foreach (var collider in Colliders)
        {
            var colliderB = collider.Value;
            var colliderBPos = colliderB.transform.position;
            
            if (colliderA == colliderB)
                continue;

            var dirAB = colliderBPos - colliderAPos;
            var dirANP = nextPosition - colliderAPos;

            if (Vector3.Dot(dirAB, dirANP) <= 0)
                continue;

            if (colliderA.Radius + colliderB.Radius >= Vector3.Distance(colliderBPos, nextPosition))
            {
                var AB = colliderAPos - colliderBPos;
                var unitAB = AB.normalized;
                var rSumAB = colliderA.Radius + colliderB.Radius;

                nextPosition.x = colliderBPos.x + (rSumAB + .07f) * unitAB.x;
                nextPosition.y = colliderBPos.y + (rSumAB + .07f) * unitAB.y;

                //nextPosition = go.transform.position;
            }
        }

        WorldBounds.KeepInBounds(ref nextPosition, colliderA.Radius);

        return nextPosition;
    }

    public static bool PointOverlap(CircleCollider collider, Vector3 point)
    {
        if (Vector3.Distance(point, collider.transform.position) <= collider.Radius)
            return true;
        else
            return false;
    }
}
