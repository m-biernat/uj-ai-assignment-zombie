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

    public static bool IsColliding(GameObject go, Vector3 nextPosition)
    {
        var collision = false;
        var colliderA = Colliders[go];

        foreach (var collider in Colliders)
        {
            var colliderB = collider.Value;
            
            if (colliderA == colliderB)
                continue;

            var colliderPositon = collider.Key.transform.position;

            if (colliderA.Radius + colliderB.Radius >= Vector3.Distance(colliderPositon, nextPosition))
                collision = true;
        }

        collision |= WorldBounds.CheckInBounds(nextPosition, colliderA.Radius);

        return collision;
    }
}
