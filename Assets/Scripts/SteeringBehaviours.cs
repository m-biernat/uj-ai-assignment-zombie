using UnityEngine;

public class SteeringBehaviours
{
    Agent _agent;

    public SteeringBehaviours(Agent owner)
    {
        _agent = owner;
    }

    public Vector3 Calculate()
    {
        //return Vector3.zero;
        return Seek(_agent.enemy.transform.position);
    }

    public Vector3 Seek(Vector3 targetPosition)
    {
        var desiredVelocity = Vector3.Normalize(targetPosition - _agent.transform.position)
                            * _agent.MaxSpeed;

        return (desiredVelocity - _agent.Velocity);
    }

    public Vector3 Flee(Vector3 targetPosition)
    {
        var dir = _agent.transform.position - targetPosition;
        
        if (Vector3.SqrMagnitude(dir) > _agent.FleePanicDistance)
        {
            return Vector3.zero;
        }

        var desiredVelocity = Vector3.Normalize(dir)
                            * _agent.MaxSpeed;

        return (desiredVelocity - _agent.Velocity);
    }

    public enum Deceleration { slow = 3, normal = 2, fast = 1 }

    public Vector3 Arrive(Vector3 targetPosition, Deceleration deceleration)
    {
        var toTarget = targetPosition - _agent.transform.position;

        var dist = toTarget.magnitude;

        if (dist > 0)
        {
            var speed = dist / ((float)deceleration * _agent.DecelerationTweaker);

            speed = Mathf.Min(speed, _agent.MaxSpeed);

            var desiredVelocity = toTarget * speed / dist;

            return (desiredVelocity - _agent.Velocity);
        }

        return Vector3.zero;
    }
}
