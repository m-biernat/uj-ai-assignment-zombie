using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField]
    GameObject _gameObject;

    public static Transform Transform { get; private set; }

    public static Vector3 Position { get => Transform.position; }

    public static Vector3 Heading { get => Transform.up; }

    public static Vector3 Side { get => Transform.right; }

    private static IVelocity _velocityComponent;

    public static Vector3 Velocity { get => _velocityComponent.Velocity; }

    void OnAwake() => SetTarget();

    void OnValidate() => SetTarget();

    void SetTarget() 
    {
        Transform = _gameObject?.transform;
        _velocityComponent = _gameObject?.GetComponent<IVelocity>();
    }
}
