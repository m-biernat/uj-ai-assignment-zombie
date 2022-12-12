using UnityEngine;

public class AgentController : MonoBehaviour, IVelocity
{
    public Vector3 Velocity { get; private set; }

    [field: SerializeField]
    public float Mass { get; private set; } = 1.0f;

    [field: SerializeField]
    public float MaxSpeed { get; private set; } = 1.0f;

    [field: SerializeField, Space]
    public float ArriveDistance { get; private set; } = 1.0f;

    [field: SerializeField]
    public float FleePanicDistance { get; private set; } = 1.0f;

    [field: SerializeField, Space]
    public float WanderRadius { get; private set; } = 1.0f;

    [field: SerializeField]
    public float WanderDistance { get; private set; } = 1.0f;

    [field: SerializeField]
    public float WanderJitter { get; private set; } = 1.0f;

    Vector3 _wanderTarget = Vector3.zero;

    void Update()
    {
        var steeringForce = CalculateForces();

        var acceleration = steeringForce / Mass;

        Velocity += acceleration * Time.deltaTime;

        Velocity = Vector2.ClampMagnitude(Velocity, MaxSpeed);

        var nextPosition = transform.position + Velocity * Time.deltaTime;

        transform.position = CircleCollider.ResolveCollision(gameObject, nextPosition);

        //transform.rotation = Quaternion.LookRotation(Vector3.forward, Velocity * Time.deltaTime);
    }

    Vector3 CalculateForces() 
    {
        //return Seek(Target.Position);
        return Wander();
    }

    Vector3 Seek(Vector3 targetPosition)
    {
        var targetDir = targetPosition - transform.position;

        //if (targetDir.sqrMagnitude < ArriveDistance) 
        //    return -Velocity;   // return Vector3.zero;

        var desiredVelocity = targetDir.normalized
                            * MaxSpeed;

        return (desiredVelocity - Velocity);
    }

    Vector3 Flee(Vector3 targetPosition)
    {
        var targetDir = transform.position - targetPosition;
        
        //if (targetDir.sqrMagnitude > FleePanicDistance)
        //    return Vector3.zero;

        var desiredVelocity = targetDir.normalized
                            * MaxSpeed;

        return (desiredVelocity - Velocity);
    }

    Vector3 Pursue()
    {
        var targetDir = Target.Position - transform.position;

        var targetSpeed = Target.Velocity.magnitude;

        if (targetSpeed < 0.01f)
            return Seek(Target.Position);

        float lookAhead = targetDir.magnitude / (MaxSpeed + targetSpeed);
        var targetPosition = Target.Position + Target.Velocity.normalized * lookAhead;

        return Seek(targetPosition);
    }

    Vector3 Evade()
    {
        var targetDir = Target.Position - transform.position;

        float lookAhead = targetDir.magnitude / (MaxSpeed + Target.Velocity.magnitude);
        var targetPosition = Target.Position + Target.Velocity.normalized * lookAhead;

        return Flee(targetPosition);
    }

    Vector3 Wander()
    {
        _wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * WanderJitter,
                                     Random.Range(-1.0f, 1.0f) * WanderJitter, 0);

        _wanderTarget.Normalize();
        _wanderTarget *= WanderRadius;

        var targetLocal = _wanderTarget + Velocity.normalized * WanderDistance;
        var targetWorld = transform.TransformPoint(targetLocal);

        //Debug.DrawLine(transform.position, targetWorld, Color.cyan);

        return Seek(targetWorld);
    }
}
