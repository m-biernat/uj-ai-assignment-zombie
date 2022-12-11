using UnityEngine;

[RequireComponent(typeof(CircleCollider))]
public abstract class MovingEntity : MonoBehaviour
{
    public CircleCollider Collider { get; protected set; }

    public Vector3 Velocity { get; protected set; }

    public Vector3 Heading { get; protected set; }

    public Vector3 Side { get; protected set; }

    [field: SerializeField]
    public float Mass { get; protected set; } = 1.0f;

    [field: SerializeField]
    public float MaxSpeed { get; protected set; } = 1.0f;

    [field: SerializeField]
    public float MaxForce { get; protected set; } = 1.0f;

    [field: SerializeField]
    public float MaxTurnRate { get; protected set; } = 1.0f;

    protected virtual void Awake() => Collider = GetComponent<CircleCollider>();
}
