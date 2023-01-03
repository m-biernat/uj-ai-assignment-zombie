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

    [field: SerializeField]
    public float WanderBoundsMargin { get; private set; } = .5f;


    [field: SerializeField, Space, Header("Hide behaviour")]
    public float DistanceFromObstacle { get; private set; } = 1.0f;

    System.Nullable<Vector3> _hidingSpot;

    public bool MayBeVisible { get; private set; }

    public bool Exposed { get; private set; }

    [field: SerializeField]
    public float HidingDuration { get; private set; } = 1.5f;

    float _hidingDuration = 0.0f;

    
    [field: SerializeField, Range(0, 1), Space, Header("Flock behaviour")]
    public int FlockingEnabled { get; private set; }

    [field: SerializeField]
    public float NeighbourRadius { get; private set; }

    [field: SerializeField]
    public float SeparationRadius { get; private set; }

    [field: SerializeField]
    public float AlignmentAmount { get; private set; }

    [field: SerializeField]
    public float CohesionAmount { get; private set; }

    [field: SerializeField]
    public float SeparationAmount { get; private set; }


    [field: SerializeField, Space, Header("Obstacle avoidance")]
    public float DetectionDistance { get; private set; } = 1.0f;

    [field: SerializeField]
    public float FeelerAngle { get; private set; } = 45.0f;

    [field: SerializeField]
    public float AvoidanceForce { get; private set; } = .5f;

    [field: SerializeField]
    public float BreakingForce { get; private set; } = .5f;


    [field: SerializeField, Space, Header("Collisions")]
    public bool EnableCollsions { get; private set; } = true;

    void Awake() => _agent = GetComponent<Agent>();

    void Update()
    {
        VisibilityCheck();

        var steeringForce = CalculateForces();

        var acceleration = steeringForce / Mass;

        Velocity += acceleration * Time.deltaTime;

        Velocity = Vector2.ClampMagnitude(Velocity, MaxSpeed);

        var nextPosition = transform.position + Velocity * Time.deltaTime;

        if (EnableCollsions)
            nextPosition = CircleCollider.ResolveCollision(gameObject, nextPosition);

        transform.position = nextPosition;

        //Debug.DrawLine(transform.position, transform.position + Velocity.normalized, Color.red);

        //transform.rotation = Quaternion.LookRotation(Vector3.forward, Velocity * Time.deltaTime);
    }

    Vector3 CalculateForces() 
    {
        var dist = Vector3.Distance(transform.position, Target.Position);

        if (Exposed) 
        {
            if (dist < EvadeMinDistance)
                return Evade() + AvoidObstacles();

            if (MayBeVisible) {
                TurnHiding();
            }
            /*
            else
                if (dist < PursueMinDistance)
                    return Pursue();
            */
        }

        if (Hiding())
            return Hide() + AvoidObstacles();

        var forces = Wander() + FlockingEnabled * Flock() + AvoidObstacles();

        return forces;
    }

    void TurnHiding() => _hidingDuration = HidingDuration;

    bool Hiding()
    {
        if (_hidingDuration > 0) {
            _hidingDuration -= Time.deltaTime;
            return true;
        }

        return false;
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
        WorldBounds.KeepInBounds(ref targetWorld, WanderBoundsMargin);

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

    Vector3 AvoidObstacles()
    {
        var heading = Velocity.normalized;
        var feeler = heading * DetectionDistance;
        
        var fp = feeler + transform.position;
        var lp = Quaternion.Euler(0, 0, FeelerAngle) * feeler + transform.position;
        var rp = Quaternion.Euler(0, 0, -FeelerAngle) * feeler + transform.position;

        //Debug.DrawLine(transform.position, fp, Color.red);
        //Debug.DrawLine(transform.position, lp, Color.green);
        //Debug.DrawLine(transform.position, rp, Color.blue);

        var fc = WorldBounds.CheckInBounds(fp);
        var lc = WorldBounds.CheckInBounds(lp);
        var rc = WorldBounds.CheckInBounds(rp);

        foreach (var collider in CircleCollider.Colliders)
        {
            if (collider.Key == gameObject)
                continue;

            fc |= CircleCollider.PointOverlap(collider.Value, fp);
            lc |= CircleCollider.PointOverlap(collider.Value, lp);
            rc |= CircleCollider.PointOverlap(collider.Value, rp);
        }

        var avoidanceDir = Vector3.zero;
        var x = 0;
        
        if (fc && lc && rc) 
            return Seek(-heading * AvoidanceForce + transform.position);

        if (fc) {
            //Debug.DrawLine(transform.position, fp, Color.red);
            avoidanceDir += -heading * BreakingForce;
            x++;
        }

        if (lc) {
            //Debug.DrawLine(transform.position, lp, Color.green);
            avoidanceDir += Quaternion.Euler(0, 0, -90) * heading * AvoidanceForce;
            x++;
        }
        
        if (rc) {
            //Debug.DrawLine(transform.position, rp, Color.blue);
            avoidanceDir += Quaternion.Euler(0, 0, 90) * heading * AvoidanceForce;
            x++;
        }
        
        if (x > 0)
            avoidanceDir /= x;

        if (avoidanceDir.sqrMagnitude > 0)
            avoidanceDir = Seek(avoidanceDir + transform.position);

        //Debug.DrawRay(transform.position, avoidanceDir, Color.yellow);

        return avoidanceDir;
    }
}
