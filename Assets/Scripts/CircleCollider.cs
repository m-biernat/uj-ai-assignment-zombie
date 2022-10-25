using UnityEngine;
using System.Collections.Generic;

public class CircleCollider : MonoBehaviour
{
    [SerializeField]
    private float _radius;

    private static Dictionary<GameObject, CircleCollider> _colliders;

    void Awake() 
    {
        _radius = transform.localScale.x * 0.5f;

        if (_colliders == null)
            _colliders = new Dictionary<GameObject, CircleCollider>();
        
        _colliders.Add(gameObject, this);
    }

    public static bool IsColliding(GameObject go, Vector3 nextPosition)
    {
        var collision = false;
        var colliderA = _colliders[go];

        foreach (var collider in _colliders)
        {
            var colliderB = collider.Value;
            
            if (colliderA == colliderB)
                continue;

            var colliderPositon = collider.Key.transform.position;

            if (colliderA._radius + colliderB._radius >= Vector3.Distance(colliderPositon, nextPosition))
                collision = true;
        }

        collision |= WorldBounds.CheckInBounds(nextPosition, colliderA._radius);

        return collision;
    }
}
