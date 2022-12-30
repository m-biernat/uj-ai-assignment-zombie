using UnityEngine;

public class AgentController : MonoBehaviour, IVelocity
{
    Agent _agent;

    public Vector3 Velocity { get; private set; }

    [field: SerializeField]
    public float Mass { get; private set; } = 1.0f;

    [field: SerializeField]
    public float MaxSpeed { get; private set; } = 1.0f;

    [field: SerializeField, Space, Header("Basic behaviours")]
    public float ArriveDistance { get; private set; } = 1.0f;

    [field: SerializeField]
    public float EvadeMinDistance { get; private set; } = 1.0f;

    [field: SerializeField]
    public float PursueMinDistance { get; private set; } = 1.0f;

    [field: SerializeField, Space, Header("Wander behaviour")]
    public float WanderRadius { get; private set; } = 1.0f;

    [field: SerializeField]
    public float WanderDistance { get; private set; } = 1.0f;

    [field: SerializeField]
    public float WanderJitter { get; private set; } = 1.0f;

    Vector3 _wanderTarget = Vector3.zero;

    [field: SerializeField, Space, Header("Hide behaviour")]
    public float DistanceFromObstacle { get; private set; } = 1.0f;

    System.Nullable<Vector3> _hidingSpot;

    public bool MayBeVisible { get; private set; }

    public bool Exposed { get; private set; }

    
    [field: SerializeField, Space, Header("Flock behaviour")]
    public float NeighbourRadius { get; private set; }

    [field: SerializeField]
    public float SeparationRadius { get; private set; }

    [field: SerializeField]
    public float AlignmentAmount { get; private set; }

    [field: SerializeField]
    public float CohesionAmount { get; private set; }

    [field: SerializeField]
    public float SeparationAmount { get; private set; }

    void Awake() => _agent = GetComponent<Agent>();

    void Update()
    {
        VisibilityCheck();

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
        var dist = Vector3.Distance(transform.position, Target.Position);

        if (Exposed) 
        {
            if (MayBeVisible)
            {   
                if (dist < EvadeMinDistance)
                    return Evade();

                return Hide();
            }
            /*
            else
                if (dist < PursueMinDistance)
                    return Pursue();
            */
        }

        return Wander() + Flock();
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
        
        //if (targetDir.sqrMagnitude < FleePanicDistance)
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

    Vector3 Hide()
    {
        if (Target.Velocity.sqrMagnitude > 0.01f || _hidingSpot == null)
            ChooseHidingSpot();
        
        return Seek(_hidingSpot.Value);
    }

    void ChooseHidingSpot()
    {
        var minDist = Mathf.Infinity;
        
        foreach (var obstaclePosition in Obstacle.Positions)
        {
            var hideDir = obstaclePosition - Target.Position;
            var hidePos = obstaclePosition + hideDir.normalized * DistanceFromObstacle;

            var dist = Vector3.Distance(transform.position, hidePos);

            if (dist < minDist)
            {
                minDist = dist;
                _hidingSpot = hidePos;
            }
        }
    }

    void VisibilityCheck()
    {
        var agentDir = transform.position - Target.Position;

        if (Vector3.Dot(agentDir, Target.Heading) > 0)
            MayBeVisible = true;
        else
            MayBeVisible = false;

        GameObject hit;
        if (Ray.Cast(gameObject, transform.position, -agentDir, out hit)
            && hit.CompareTag("Player"))
            Exposed = true;
        else
            Exposed = false;
    }

    Vector3 Flock()
    {
        var flocking = Vector3.zero;
        
        var neighbourCount = 0;
        var separatedCount = 0;
        
        var alignment = Vector3.zero;
        var cohesion = Vector3.zero;
        var separation = Vector3.zero;

        foreach (var other in Agent.Collection)
        {
            if (other == _agent)
                continue;

            var dist = Vector3.Distance(transform.position, other.transform.position);

            if (dist < NeighbourRadius)
            {
                alignment += other.Controller.Velocity;
                cohesion += other.transform.position;

                if (dist < SeparationRadius)
                {
                    var diff = transform.position - other.transform.position;
                    separation += diff.normalized / diff.magnitude;

                    separatedCount++;
                }

                neighbourCount++;
            }
        }

        if (neighbourCount > 0)
        {
            alignment /= neighbourCount;
            alignment = alignment.normalized * AlignmentAmount;

            cohesion /= neighbourCount;
            cohesion -= transform.position;
            cohesion = cohesion.normalized * CohesionAmount;

            if (separatedCount > 0)
            {
                separation /= separatedCount;
                separation *= SeparationAmount;
            }

            flocking = alignment + cohesion + separation; // * MaxSpeed?
        }

        return flocking;
    }
}
