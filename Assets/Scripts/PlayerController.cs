using UnityEngine;

public class PlayerController : MonoBehaviour, IVelocity
{
    [field: SerializeField]
    public float Speed { get; private set; } = 5.0f;

    public Vector3 Velocity { get; private set; }

    Camera _cam;

    void Start() => _cam = Camera.main;

    void Update()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");
        
        Move(h, v);

        var mousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0.0f;
        var dir = (mousePosition - transform.position).normalized;

        Debug.DrawLine(transform.position, mousePosition);
        Debug.DrawRay(transform.position, dir, Color.red);

        Rotate(dir);

        if (Input.GetMouseButtonDown(0))
            Shoot(dir);
    }

    void Move(float h, float v)
    {
        var dir = new Vector3(h, v, 0);

        if (dir.magnitude == 0)
        {
            Velocity = Vector3.zero;
            return;
        }
        
        var nextPosition = transform.position + dir.normalized * Speed * Time.deltaTime;

        var position = CircleCollider.ResolveCollision(gameObject, nextPosition);

        Velocity = position - transform.position;

        transform.position = position;
    }

    void Rotate(Vector3 dir) 
        => transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);

    void Shoot(Vector3 dir)
    {
        GameObject hit;
        if (Ray.Cast(gameObject, transform.position, dir, out hit) 
            && hit.CompareTag("Zombie"))
            {
                Debug.Log(hit.name + " has been shot!");
                Destroy(hit);
            }
    }
}
