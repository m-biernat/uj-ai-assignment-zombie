using UnityEngine;

public class Agent : MovingEntity
{
    SteeringBehaviours _steering;

    [field: SerializeField]
    public float FleePanicDistance { get; protected set; } = 10.0f;

    [field: SerializeField]
    public float DecelerationTweaker { get; protected set; } = 0.3f;

    [field: SerializeField]
    public float WanderRadius { get; protected set; }

    [field: SerializeField]
    public float WanderDistance { get; protected set; }

    [field: SerializeField]
    public float WanderJitter { get; protected set; }

    public Transform enemy;

    protected override void Awake()
    {
        base.Awake();
        _steering = new SteeringBehaviours(this);
    }

    void Update()
    {
        var steeringForce = _steering.Calculate();

        var acceleration = steeringForce / Mass;

        Velocity += acceleration * Time.deltaTime;

        Velocity = Vector2.ClampMagnitude(Velocity, MaxSpeed);

        transform.position += Velocity * Time.deltaTime;

        if (Velocity.sqrMagnitude > 0.00000001f)
        {
            Heading = Velocity.normalized;
            Side = Vector2.Perpendicular(Heading);
        }
    }
}
